using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Genesis.Input.MySqlDb
{
    public class MySqlDbInput : InputExecutor
    {
        private string _query = @"
                                SELECT
	                                tab.TABLE_NAME as 'Table',
                                    col.COLUMN_NAME as 'ColumnName',
                                    col.COLUMN_DEFAULT as 'DefaultValue',
                                    col.IS_NULLABLE as 'Nullable',
                                    col.DATA_TYPE as 'DbDataType',
                                    col.CHARACTER_MAXIMUM_LENGTH as 'MaxLength',
                                    col.COLUMN_KEY as 'KeyInfo',
                                    col.EXTRA as 'Flags'
                                FROM INFORMATION_SCHEMA.tables tab
                                INNER JOIN INFORMATION_SCHEMA.columns col 
                                    ON col.TABLE_NAME = tab.TABLE_NAME
                                WHERE 
	                                tab.TABLE_TYPE = 'BASE TABLE' 
                                AND tab.TABLE_SCHEMA = '{DB}'
                                ORDER BY tab.TABLE_NAME;
                                ";

        public override string CommandText => "mysql";
        public override string FriendlyName => "MySQL Server DB";
        public override string Description => "Populate the object graph with the schema of a MySQL Server database";

        public MySqlConfig Config { get; set; } = null!;

        protected override void OnInitialized()
        {
            Config = (MySqlConfig)Configuration;
            _query = _query.Replace("{DB}", Config.Database);
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

            var rows = GetSchemaGraph();

            Text.BlueLine($"Found {rows.Count} possible objects. Matching exclusions...");

            foreach(var i in rows)
            {
                if (isExcluded(i, Config.ExcludePrefixes))
                    continue;

                if(genesis.Objects.SingleOrDefault(x => x.KeyId == i.KeyId) == null) // only add if it doesn't exist
                {
                    Text.White("Found "); Text.Yellow($"Table:{i.Name}, Key:{i.KeyId}, Type:{i.KeyDataType}, - ");

                    //if(keyCount > 0)
                    //    i.Relationships.AddRange(keys.Where(x => x.ForeignTable == i.Name || x.PrimaryTable == i.Name).ToList());
                    
                    await genesis.AddObject(i); //yeah, this can blow up - leaving it for errors
                }
            }

            return await Task.FromResult(new InputGenesisExecutionResult {
                Success = true,
            });
        }

        //private List<RelationshipGraph> GetRelationships()
        //{
        //    var list = new List<RelationshipGraph>();
        //    using var con = new SqlConnection(Config.ConnectionString);
        //    using var cmd = new SqlCommand(Q_FOREIGN_KEYS, con);

        //    con.Open();

        //    using var r = cmd.ExecuteReader(CommandBehavior.CloseConnection);

        //    while(r.Read())
        //    {
        //        list.Add(new RelationshipGraph { 
        //            ForeignColumn = r["ForeignColumn"].ToString(),
        //            PrimaryColumn = r["PrimaryColumn"].ToString(),
        //            ForeignTable = r["ForeignTable"].ToString(),
        //            PrimaryTable = r["PrimaryTable"].ToString()
        //        });
        //    }

        //    return list;
        //}

        /// <summary>
        /// This is ugly, acknowledged
        /// </summary>
        /// <returns></returns>
        private List<ObjectGraph> GetSchemaGraph()
        {
            static T getValue<T>(MySqlDataReader rdr, string fieldName) where T: new()
                => rdr.IsDBNull(fieldName)
                    ? new T()
                    : rdr.GetFieldValue<T>(fieldName);

            var objs = new List<ObjectGraph>();
            using (var con = new MySqlConnection(Config.ToConnectionString()))
            {
                using var r = new MySqlCommand(_query, con);
                con.Open();

                using var rdr = r.ExecuteReader(CommandBehavior.CloseConnection);
                if (!rdr.HasRows)
                    return objs;

                objs.Clear();

                var list = new List<SchemaRecord>();

                var temp = new SchemaRecord();
                while (rdr.Read())
                {
                    try
                    {
                        if (rdr["Table"]?.ToString()?.StartsWith("_") ?? true) //just removing EF migrations etc, should make a toggle
                            continue;

                        temp = new SchemaRecord();
                        temp.TableName = rdr["Table"]?.ToString() ?? string.Empty;
                        temp.DbDataType = rdr["DbDataType"]?.ToString() ?? string.Empty;
                        temp.ColumnName = rdr["ColumnName"]?.ToString() ?? string.Empty;
                        temp.DefaultValue = rdr["DefaultValue"]?.ToString() ?? string.Empty;
                        temp.IsNullable = (rdr["DefaultValue"]?.ToString() ?? string.Empty).Equals("YES", StringComparison.InvariantCultureIgnoreCase);
                        temp.MaxLength = getValue<long>(rdr, "MaxLength");
                        temp.KeyInfo = rdr["KeyInfo"].ToString() ?? string.Empty;
                        temp.Flags = rdr["Flags"].ToString() ?? string.Empty;
                        
                        list.Add(temp);
                    }
                    catch(Exception ex)
                    {
                        Debug.WriteLine($"Exception building MySQL SchemaRecord: {ex.GetBaseException().Message}");
                    }
                    
                }

                var tmp = new Dictionary<string, List<SchemaRecord>>();

                var t = list
                    .GroupBy(g=>g.TableName)
                    .ToDictionary(g => g.Key, g=>g.ToList());

                foreach(var key in t.Keys) // table loop
                {
                    var val = t[key];
                    var keyInfo = val.Where(x => !string.IsNullOrEmpty(x.KeyInfo)).FirstOrDefault();

                    // finding the key, if any
                    
                    var o = new ObjectGraph
                    {
                        KeyId = (!string.IsNullOrEmpty(keyInfo?.KeyInfo) && keyInfo.KeyInfo.Equals("PRI", StringComparison.InvariantCultureIgnoreCase)) ? keyInfo.ColumnName : string.Empty,
                        KeyDataType = (!string.IsNullOrEmpty(keyInfo?.KeyInfo) && keyInfo.KeyInfo.Equals("PRI", StringComparison.InvariantCultureIgnoreCase)) ? keyInfo.DbDataType.ToCodeDataType() : string.Empty,
                        Name = val.First().TableName.FromMySqlUnderscored()
                    };

                    foreach(var p in val)
                    {
                        var prop = new PropertyGraph
                        {
                            IsKeyProperty = p.ColumnName.Equals(keyInfo?.ColumnName, StringComparison.InvariantCultureIgnoreCase),
                            Name = p.ColumnName,
                            IsNullable = p.IsNullable,
                            SourceType = p.DbDataType,
                            TypeGuess = p.DbDataType.ToCodeDataType()
                        };

                        o.Properties.Add(prop);
                    }

                    objs.Add(o);
                }
            }
            return objs;
        }

        internal sealed class SchemaRecord
        {
            public string TableName { get; set; } = string.Empty;
            public string ColumnName { get; set; } = string.Empty;
            public string DefaultValue { get; set; } = string.Empty;
            public bool IsNullable { get; set; } = false;
            public string DbDataType { get; set; } = string.Empty;
            public long MaxLength { get; set; } = 0;
            public string KeyInfo { get; set; } = string.Empty;
            public string Flags { get; set; } = string.Empty;
        }
    }
}
