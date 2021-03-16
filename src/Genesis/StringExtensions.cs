using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis
{
    //Thanks to [https://stackoverflow.com/posts/48008872/revisions] for this solution. 
    /// <summary>
    /// Parse a string into an commandline args like Main(string[] args)
    /// </summary>
    public static class StringExtensions
    {
        public static IEnumerable<string> ToArgs(this string line)
        {
            char quote = '"';
            char escape = '\\';
            bool inQuotes = false;
            bool inEscape = false;

            var arg = new StringBuilder();
            
            var argsTotal = 0; // required for "" as an arg but ditch whitespace between args

            for (int i = 0; i < line?.Length; i++)
            {
                char c = line[i];
                if (c == quote)
                {
                    argsTotal++;

                    if (inEscape)
                    {
                        arg.Append(c);       // found \" -> add " to arg
                        inEscape = false;
                    }
                    else if (inQuotes)
                        inQuotes = false;   // end quote
                    else
                        inQuotes = true;    // start quote
                }
                else if (c == escape)
                {
                    argsTotal++;

                    if (inEscape)   // encountered escape char \\
                        arg.Append(escape + escape);

                    inEscape = !inEscape;
                }
                else if (char.IsWhiteSpace(c))
                {
                    if (inQuotes)
                    {
                        argsTotal++;
                        arg.Append(c);       // append whitespace inside quote
                    }
                    else
                    {
                        if (argsTotal > 0)
                            yield return arg.ToString();

                        argsTotal = 0;
                        arg.Clear();
                    }
                }
                else
                {
                    argsTotal++;
                    if (inEscape)
                    {
                        arg.Append(escape); // encountered non-escaping backslash so add \
                        argsTotal = 0;
                        inEscape = false;
                    }
                    arg.Append(c);
                }
            }

            if (argsTotal > 0)
                yield return arg.ToString();
        }
    }
}
