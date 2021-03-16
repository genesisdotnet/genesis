using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Genesis.Cli;
using Genesis.Input;

namespace Genesis.Input.MSSqlDb
{
    public class MSSqlDbInput : InputExecutor
    {
        private const string Q_FOREIGN_KEYS = @"SELECT 
	                                                tab.[Name] AS [ForeignTable],
                                                    col.[Name] AS [ForeignColumn],
                                                    pk_tab.[Name] AS [PrimaryTable],
                                                    pk_col.[Name] AS [PrimaryColumn]
                                                FROM sys.tables tab
                                                    INNER JOIN sys.columns col 
                                                        ON col.[object_id] = tab.[object_id]
                                                    LEFT OUTER JOIN sys.foreign_key_columns fk_cols
                                                        ON fk_cols.parent_object_id = tab.[object_id]
                                                        and fk_cols.parent_column_id = col.column_id
                                                    LEFT OUTER JOIN sys.foreign_keys fk
                                                        ON fk.[object_id] = fk_cols.cONstraint_object_id
                                                    LEFT OUTER JOIN sys.tables pk_tab
                                                        ON pk_tab.[object_id] = fk_cols.referenced_object_id
                                                    LEFT OUTER JOIN sys.columns pk_col
                                                        ON pk_col.column_id = fk_cols.referenced_column_id
                                                        and pk_col.[object_id] = fk_cols.referenced_object_id
                                                WHERE 
	                                                pk_tab.[Name] IS NOT NULL
	                                                AND LEFT(tab.[Name], 6) <> 'AspNet' 
                                                ORDER BY
	                                                tab.[Name]";

        private const string Q_TABLE_PROPERTIES = @"SELECT c.*, t.[name] AS [SqlTypeName] FROM [sys].[Columns] c " +
                                                    "INNER JOIN [sys].[types] t  ON t.system_type_id = c.system_type_id " +
                                                    "WHERE c.[object_id] = @objectID " +
                                                    "ORDER BY [c].[column_id] ASC";

        public override string CommandText => "mssql";
        public override string FriendlyName => "Sql Server LocalDB";
        public override string Description => "Populate the object graph with the schema of a Microsoft SQL Server database";

        public SqlConfig Config { get; set; }

        protected override void OnInitialized(/*, string[] args */) //TODO: Pass args to the init 
        {
            Config = (SqlConfig)Configuration; //TODO: configuration is wonky
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            static bool isExcluded(ObjectGraph g, string[] ex)
            {
                foreach (var s in ex)
                    if (g.Name.StartsWith(s, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                
                return false;
            }

            var tmp = GetSchema();
            var keys = GetRelationships();

            Text.BlueLine($"Found {tmp.Count} possible objects. Matching exclusions...");

            int keyCount = 0;

            foreach(var i in tmp)
            {
                if (isExcluded(i, Config.ExcludePrefixes))
                    continue;

                
                if(genesis.Objects.SingleOrDefault(x => x.KeyId == i.KeyId) == null)
                {
                    keyCount = keys.Where(x => x.ForeignTable == i.Name || x.PrimaryTable == i.Name).Count();
                    
                    Text.White("Found "); Text.Yellow($"Table:{i.Name}, ForeignKeys:{keyCount}, Type:{i.SourceType}, - ");

                    if(keyCount > 0)
                        i.Relationships.AddRange(keys.Where(x => x.ForeignTable == i.Name || x.PrimaryTable == i.Name).ToList());
                    
                    await genesis.AddObject(i); //yeah, this can blow up - leaving it for errors
                }
            }

            return await Task.FromResult(new InputGenesisExecutionResult {
                Success = true,
            });
        }

        private List<RelationshipGraph> GetRelationships()
        {
            var list = new List<RelationshipGraph>();
            using var con = new SqlConnection(Config.ConnectionString);
            using var cmd = new SqlCommand(Q_FOREIGN_KEYS, con);

            con.Open();

            using var r = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            while(r.Read())
            {
                list.Add(new RelationshipGraph { 
                    ForeignColumn = r["ForeignColumn"].ToString(),
                    PrimaryColumn = r["PrimaryColumn"].ToString(),
                    ForeignTable = r["ForeignTable"].ToString(),
                    PrimaryTable = r["PrimaryTable"].ToString()
                });
            }

            return list;
        }

        /// <summary>
        /// This is ugly, acknowledged
        /// </summary>
        /// <returns></returns>
        private List<ObjectGraph> GetSchema()
        {
            var objs = new List<ObjectGraph>();
            using (var con = new SqlConnection(Config.ConnectionString))
            {
                using var r = new SqlCommand("SELECT * FROM [sys].[Tables] WHERE [type_desc] = 'USER_TABLE'", con);
                con.Open();

                using var rdr = r.ExecuteReader(CommandBehavior.CloseConnection);
                if (!rdr.HasRows)
                    return objs; //TODO: This could suck, user won't see anything new. Needs message stating no rows.

                objs.Clear();

                while (rdr.Read())
                {
                    if (rdr["name"].ToString().StartsWith("_")) //just removing EF migrations etc, should make a toggle
                        continue;

                    objs.Add(new ObjectGraph
                    {
                        Name = rdr["name"].ToString(),
                        SourceType = rdr["type"].ToString(),
                        KeyId = int.Parse(rdr["object_id"].ToString()),
                    });
                }
            }
            foreach (var obj in objs)
            {
                using var con = new SqlConnection(Config.ConnectionString);

                con.Open();

                using var cmd = new SqlCommand(Q_TABLE_PROPERTIES, con);

                cmd.Parameters.AddWithValue("@objectID", obj.KeyId);

                if (con.State != ConnectionState.Open)
                    con.Open();

                using var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                if (!rdr.HasRows)
                    continue;

                obj.Properties.Clear();

                while (rdr.Read())
                {
                    if (rdr["name"].ToString().StartsWith("_")) //migrations-ish table(s), skip them
                        continue; //TODO: MSSql table process - make this configurable

                    var p = new PropertyGraph
                    {
                        Name = rdr["name"].ToString(),
                        SourceType = rdr["SqlTypeName"].ToString(),
                        TypeGuess = Extensions.ToCodeDataType(rdr["SqlTypeName"].ToString()),
                        //ColumnID = rdr.GetInt32(rdr.GetOrdinal("column_id")), //don't need yet?
                        IsNullable = rdr.GetBoolean(rdr.GetOrdinal("is_nullable")),
                        //IsRowGuid = rdr.GetBoolean(rdr.GetOrdinal("is_rowguidcol")),
                        IsKeyProperty = rdr.GetBoolean(rdr.GetOrdinal("is_identity")),
                        //IsComputed = rdr.GetBoolean(rdr.GetOrdinal("is_computed")),
                        //MaxLength = rdr.GetInt16(rdr.GetOrdinal("max_length")),
                        //Precision = rdr.GetByte(rdr.GetOrdinal("precision")),
                        //Scale = rdr.GetByte(rdr.GetOrdinal("scale")),
                        //SystemTypeID = rdr.GetByte(rdr.GetOrdinal("system_type_id")),
                        //ObjectID = rdr.GetInt32(rdr.GetOrdinal("object_id")),
                    };

                    obj.Properties.Add(p);

                    if (p.IsKeyProperty)
                        obj.KeyDataType = p.SourceType;
                }
            }
            return objs;
        }
    }
}
