using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis.Output.AspNetMvcController
{
    public static class Extensions
    {
        /// <summary>
        /// Returns a list of interfaces and parameter names seperated by comma for constructor injection
        /// </summary>
        /// <param name="interfaces">List of interface names.</param>
        /// <returns>A comma delimited list of interfaces for a constructor.</returns>
        public static string ToInjectionString(this List<string> interfaces)
        {
            var sb = new StringBuilder();

            interfaces.ForEach(i =>
            {
                var tmp = i.ToLower();
                sb.AppendLine("\t\t\t" + i + " " + tmp[1] + i[2..] + ", ");
            });

            var output = sb.ToString();
            if (!string.IsNullOrEmpty(output))
                output = output[0..^2]; // skip ', ' at the end

            return output;
        }

        /// <summary>
        /// Returns a string containing line seperated class member declarations.
        /// </summary>
        /// <param name="interfaces">List of interface names.</param>
        /// <returns>A string containing line seperated member declarations.</returns>
        public static string ToInjectionMembersString(this List<string> interfaces)
        {
            var sb = new StringBuilder();

            interfaces.ForEach(i =>
            {
                var tmp = i.ToLower();
                sb.AppendLine("\t\t" + i + " _" + tmp[1] + i[2..] + " = default;");
            });

            return sb.ToString();
        }

        /// <summary>
        /// Returns a string containing line seperated parameter to member assignments.
        /// </summary>
        /// <param name="interfaces">List of interface names.</param>
        /// <returns>A string containing line seperated parameter to member assignments.</returns>
        public static string ToInjectionAssignmentsString(this List<string> interfaces)
        {
            var sb = new StringBuilder();

            interfaces.ForEach(i =>
            {
                var tmp = i.ToLower();
                sb.AppendLine("\t\t\t_" + tmp[1] + i[2..] + " = " + tmp[1] + i[2..] + ";");
            });

            return sb.ToString();
        }
    }
}
