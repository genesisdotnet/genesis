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
using System.Text.RegularExpressions;

namespace Genesis
{
    public static class Extensions
    {
        private static readonly Regex _nullableRegex = new Regex("(?<=<)[^>]*(?=>)"); // all text between <>, could be improved I'm sure

        /// <summary>
        /// Returns a c# parseable name for a type, beautifying generics.
        /// <example>List{Type}</example>
        /// </summary>
        /// <param name="type">The <see cref="Type"/> whose name may need formatted for generic types.</param>
        /// <returns>a c# parseable string representing the <see cref="Type"/> passed in.</returns>
        public static string GetFormattedName(this Type type, bool useNullableReferenceTypes = false)
        {
            if (!type.IsGenericType)
                return type.Name;

            var sb = new StringBuilder();
            sb.Append(type.Name.Substring(0, type.Name.LastIndexOf("`", StringComparison.Ordinal)));
            sb.Append(type.GetGenericArguments()
                .Aggregate("<",(aggregate, innerType) => aggregate + (aggregate == "<" ? "" : ",") + GetFormattedName(innerType)));
            sb.Append(">");

            var result = sb.ToString();

            if (useNullableReferenceTypes)
                return _nullableRegex.IsMatch(result)
                    ? _nullableRegex.Match(result).Value + "?" // Returns Whatever?
                    : result;

            return sb.ToString(); // Returns Nullable<Whatever>
        }

        /// <summary>
        /// Converts a string containing a variable name to correct casing. Ex.   string firstName = string.empty; from FirstName
        /// </summary>
        /// <param name="variableString">string containing a 'VariableName'</param>
        /// <returns>string containing a formatted 'variableName'</returns>
        public static string ToCorrectedCase(this string variableString)
            => (variableString.Length > 2) //longer than two letter properties, like ID
#pragma warning disable IDE0057 // Use range operator
                ? variableString.ToLower(CultureInfo.CurrentCulture)[0] + variableString.Substring(1, variableString.Length - 1)
#pragma warning restore IDE0057 // Use range operator
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
                case "string":
                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":                    
                case "text":
                case "ntext":
                    return "string";

                case "xml": return "Xml";

                case "nullable<tinyint>": 
                case "nullable<byte>": return "byte?";

                case "nullable<int16>": return "short?";

                case "nullable<int64>": return "long?";

                case "nullable<int32>": return "int?";

                case "nullable<boolean>": return "bool?";

                case "int32":
                case "int": return "int";

                case "long":
                case "bigint": return "long";

                case "short":
                case "smallint": return "short";

                case "byte":
                case "tinyint": return "byte";

                case "double":
                case "money": return "double";

                case "smallmoney":
                case "decimal":
                case "numeric":
                    return "decimal";

                case "guid":
                case "uniqueidentifier": return "Guid";

                case "float": return "float";

                case "boolean":
                case "bool":
                case "bit": return "bool";

                case "single":
                case "real": return "single";

                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "date":
                    return "DateTime";

                case "image":
                case "rowversion":
                case "timestamp":
                case "varbinary":
                case "varbinary(max)": //filestreams in sql
                    return "byte[]";

                default:
                    {
                        return dbDataType;
                    }
            }
        }
        /// <summary>
        /// Basic datatypes like ints, bool, string etc. *Based on Google's Protocol Buffers
        /// </summary>
        /// <param name="dbDataType"></param>
        /// <returns></returns>
        public static string ToBasicDataType(this string dbDataType)
        {
            //https://developers.google.com/protocol-buffers/docs/proto#scalar

            switch (dbDataType.ToLower()) //NOTE: Null?
            {
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "date":
                case "datetimeoffset":
                case "xml":
                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":                    //TODO: Move this somewhere sensible?
                case "text":
                case "ntext":
                case "uniqueidentifier":
                    return "string";

                case "real": return "int32";
                case "int": return "int32";
                case "bigint": return "int64";
                case "smallint": return "int32";
                case "tinyint": return "int32";
                case "money": return "double";

                case "smallmoney":
                case "decimal":
                case "numeric":
                    return "double";

                case "float": return "float";
                case "bit": return "bool";
                
                case "image":
                case "rowversion":
                case "timestamp":
                case "varbinary":
                case "varbinary(max)": //filestreams in sql
                    return "bytes";

                default:
                    {
                        return dbDataType;
                    }

            }
        }

        /// <summary>
        /// Unloads the inner <see cref="GenesisAssemblyLoadContext"/> object and optionally waits until the GC cleans it up
        /// </summary>
        /// <param name="weakRef">The current <see cref="WeakReference"/> object.</param>
        /// <param name="waitUntilFinished"><c>true</c> to wait for GC to clean up references, otherwise <c>false</c>.</param>
        public static void UnloadAssembly(this WeakReference weakRef, bool waitUntilFinished = true)
        {
            ((GenesisAssemblyLoadContext)weakRef.Target).Unload();

            if (!waitUntilFinished)
                return;

            for (var i = 0; weakRef.IsAlive && (i < 10); i++) //NOTE: Don't have to wait if we don't want to, GC will eventually get it. 
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        /// <summary>
        /// Gets the human readable type names for each type in the array. These are presumed to be generic types
        /// </summary>
        /// <param name="types">an array of generic type descriptors</param>
        /// <returns>an array of formatted type names</returns>
        public static string[] ToFormattedNames(this Type[] types) 
            => types.Select(s => s.GetFormattedName())
                .ToArray();
    }
}
