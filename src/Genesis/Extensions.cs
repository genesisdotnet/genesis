using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Composition.Hosting;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;

namespace Genesis
{
    public static class Extensions
    {
        /// <summary>
        /// Converts a string containing a variable name to correct casing. Ex.   string firstName = string.empty; from FirstName
        /// </summary>
        /// <param name="variableString">string containing a 'VariableName'</param>
        /// <returns>string containing a formatted 'variableName'</returns>
        public static string ToCorrectedCase(this string variableString)
            => (variableString.Length > 2) //longer than two letter properties, like ID
                ? variableString.ToLower(CultureInfo.CurrentCulture)[0] + variableString.Substring(1, variableString.Length - 1)
                : variableString; //TODO: This feels off return what was passed in?

        /// <summary>
        /// Returns a singular word from a plural
        /// </summary>
        /// <param name="pluralName"></param>
        /// <returns></returns>
        public static string ToSingular(this string pluralName)
            => Inflector.MakeSingular(pluralName);

        public static ContainerConfiguration WithAssembliesInPath(this ContainerConfiguration configuration, string path, SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            return WithAssembliesInPath(configuration, path, null, searchOption);
        }

        /// <summary>
        /// Gets a ContainerConfiguration with the assemblies found in the given search path
        /// </summary>
        /// <param name="configuration"><see cref="ContainerConfiguration"/></param>
        /// <param name="path">directory search path</param>
        /// <param name="conventions">ModelProvider</param>
        /// <param name="searchOption">SearchOptions</param>
        /// <param name="searchFilter">*.dll or standard search string</param>
        /// <returns></returns>
        public static ContainerConfiguration WithAssembliesInPath(this ContainerConfiguration configuration, string path, AttributedModelProvider conventions, SearchOption searchOption = SearchOption.TopDirectoryOnly, string searchFilter = "*.dll")
        {
            var assemblies = Directory
                .GetFiles(path, searchFilter, searchOption)
                .Select(AssemblyLoadContext.Default.LoadFromAssemblyPath);

            configuration = configuration.WithAssemblies(assemblies, conventions);

            return configuration;
        }

        /// <summary>
        /// Get the equivalent c# datatype for database type provided
        /// There's probably a better way
        /// </summary>
        /// <param name="dbDataType">string representation of the database type</param>
        /// <returns>c# type corresponding to the database type received</returns>
        public static string ToCodeDataType(this string dbDataType)
        {
            switch (dbDataType.ToLower()) //NOTE: Null?
            {
                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":                    //TODO: Move this somewhere sensible?
                case "text":
                case "ntext":
                    return "string";

                case "xml": return "Xml";
                case "int": return "int";
                case "bigint": return "long";
                case "smallint": return "short";
                case "tinyint": return "byte";
                case "money": return "double";

                case "smallmoney":
                case "decimal":
                case "numeric":
                    return "decimal";

                case "uniqueidentifier": return "Guid";
                case "float": return "float";
                case "bit": return "bool";
                case "real": return "single";

                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "date":
                    return "DateTime";

                case "datetimeoffset": return "DateTimeOffset";

                case "image":
                case "rowversion":
                case "timestamp":
                case "varbinary":
                case "varbinary(max)": //filestreams in sql
                    return "byte[]";

                default:
                    {
                        Debug.WriteLine($@"Unknown Database Type {dbDataType}");
                        return "UNKNOWN";
                    }

            }
        }
    }
}
