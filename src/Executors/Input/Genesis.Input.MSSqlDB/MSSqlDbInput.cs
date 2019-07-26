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
        private const string Q_TABLE_PROPERTIES = @"SELECT c.*, t.[name] AS [SqlTypeName] FROM [sys].[Columns] c " +
                                                    "INNER JOIN [sys].[types] t  ON t.system_type_id = c.system_type_id " +
                                                    "WHERE c.[object_id] = @objectID " +
                                                    "ORDER BY [c].[column_id] ASC";

        public override string CommandText => "mssql";
        public override string FriendlyName => "Sql Server LocalDB";
        public override string Description => "Populate the object graph with the schema of a Microsoft SQL Server database.";

        public SqlConfig Config { get; set; }

        protected override void OnInitilized(/*, string[] args */) //TODO: Pass args to the init 
        {
            Config = (SqlConfig)Configuration; //TODO: configuration is wonky
        }

        public override async Task<IGenesisExecutionResult> Execute(GenesisContext genesis, string[] args)
        {
            var tmp = GetSchema();

            Text.BlueLine($"{GetType().Name} created {tmp.Count} ObjectGraph(s)");

            foreach(var i in tmp)
            {
                Text.DarkYellowLine($"{i.Name}:{i.SourceType}");
                if(genesis.Objects.SingleOrDefault(x => x.KeyId == i.KeyId) == null)
                    await genesis.AddObject(i); //yeah, this can blow up - leaving it for errors
            }

            Text.CyanLine("Populated "+genesis.Objects.Count().ToString()+" object(s).");
            return await Task.FromResult(new InputGenesisExecutionResult {
                Success = true,
            });
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
                using (var r = new SqlCommand("SELECT * FROM [sys].[Tables] WHERE [type_desc] = 'USER_TABLE'", con))
                {
                    con.Open();

                    using (var rdr = r.ExecuteReader(CommandBehavior.CloseConnection))
                    {
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
                }
            }
            foreach (var obj in objs)
            {
                using (var con = new SqlConnection(Config.ConnectionString))
                {
                    con.Open();
                    using (var cmd = new SqlCommand(Q_TABLE_PROPERTIES, con))
                    {
                        cmd.Parameters.AddWithValue("@objectID", obj.KeyId);

                        if (con.State != ConnectionState.Open)
                            con.Open();

                        using (var rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                        {
                            if (!rdr.HasRows)
                                continue;

                            obj.Properties.Clear();

                            while (rdr.Read())
                            {
                                if (rdr["name"].ToString().StartsWith("_")) //migrations-ish table(s), skip them
                                    continue;

                                obj.Properties.Add(new PropertyGraph
                                {
                                    Name = rdr["name"].ToString(),
                                    SourceType = rdr["SqlTypeName"].ToString(),
                                    TypeGuess = "convert me 2 c#", //TODO: Convert this to something csharpy for the generators, yeah assuming a lot
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
                                });
                            }
                        }
                    }
                }                
            }
            return objs;
        }
    }
}
