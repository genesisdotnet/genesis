using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

/*
The MIT License(MIT)

Copyright(c) 2013 Scott Kirkland

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE GeneratorS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace Genesis
{
    public static class Inflector
    {
        private static readonly List<InflectorRule> plurals = new List<InflectorRule>();
        private static readonly List<InflectorRule> singles = new List<InflectorRule>();
        private static readonly List<string> uncountables = new List<string>();

        static Inflector()
        {
            AddPluralRule("$", "s");
            AddPluralRule("s$", "s");
            AddPluralRule("(ax|test)is$", "$1es");
            AddPluralRule("(octop|vir)us$", "$1i");
            AddPluralRule("(alias|status)$", "$1es");
            AddPluralRule("(bu)s$", "$1ses");
            AddPluralRule("(buffal|tomat)o$", "$1oes");
            AddPluralRule("([ti])um$", "$1a");
            AddPluralRule("sis$", "ses");
            AddPluralRule("(?:([^f])fe|([lr])f)$", "$1$2ves");
            AddPluralRule("(hive)$", "$1s");
            AddPluralRule("([^aeiouy]|qu)y$", "$1ies");
            AddPluralRule("(x|ch|ss|sh)$", "$1es");
            AddPluralRule("(matr|vert|ind)ix|ex$", "$1ices");
            AddPluralRule("([m|l])ouse$", "$1ice");
            AddPluralRule("^(ox)$", "$1en");
            AddPluralRule("(quiz)$", "$1zes");

            AddSingularRule("s$", string.Empty);
            AddSingularRule("ss$", "ss");
            AddSingularRule("(n)ews$", "$1ews");
            AddSingularRule("([ti])a$", "$1um");
            AddSingularRule("((a)naly|(b)a|(d)iagno|(p)arenthe|(p)rogno|(s)ynop|(t)he)ses$", "$1$2sis");
            AddSingularRule("(^analy)ses$", "$1sis");
            AddSingularRule("([^f])ves$", "$1fe");
            AddSingularRule("(hive)s$", "$1");
            AddSingularRule("(tive)s$", "$1");
            AddSingularRule("([lr])ves$", "$1f");
            AddSingularRule("([^aeiouy]|qu)ies$", "$1y");
            AddSingularRule("(s)eries$", "$1eries");
            AddSingularRule("(m)ovies$", "$1ovie");
            AddSingularRule("(x|ch|ss|sh)es$", "$1");
            AddSingularRule("([m|l])ice$", "$1ouse");
            AddSingularRule("(bus)es$", "$1");
            AddSingularRule("(o)es$", "$1");
            AddSingularRule("(shoe)s$", "$1");
            AddSingularRule("(cris|ax|test)es$", "$1is");
            AddSingularRule("(octop|vir)i$", "$1us");
            AddSingularRule("(alias|status)$", "$1");
            AddSingularRule("(alias|status)es$", "$1");
            AddSingularRule("^(ox)en", "$1");
            AddSingularRule("(vert|ind)ices$", "$1ex");
            AddSingularRule("(matr)ices$", "$1ix");
            AddSingularRule("(quiz)zes$", "$1");

            AddIrregularRule("person", "people");
            AddIrregularRule("man", "men");
            AddIrregularRule("child", "children");
            AddIrregularRule("sex", "sexes");
            AddIrregularRule("tax", "taxes");
            AddIrregularRule("move", "moves");

            AddUnknownCountRule("equipment");
            AddUnknownCountRule("information");
            AddUnknownCountRule("rice");
            AddUnknownCountRule("money");
            AddUnknownCountRule("species");
            AddUnknownCountRule("series");
            AddUnknownCountRule("fish");
            AddUnknownCountRule("sheep");
        }

        /// <summary>
        /// Adds the irregular rule.
        /// </summary>
        /// <param name="singular">The singular.</param>
        /// <param name="plural">The plural.</param>
        private static void AddIrregularRule(string singular, string plural)
        {
            AddPluralRule(string.Concat("(", singular[0], ")", singular.Substring(1), "$"), string.Concat("$1", plural.Substring(1)));
            AddSingularRule(string.Concat("(", plural[0], ")", plural.Substring(1), "$"), string.Concat("$1", singular.Substring(1)));
        }

        /// <summary>
        /// Adds the unknown count rule.
        /// </summary>
        /// <param name="word">The word.</param>
        private static void AddUnknownCountRule(string word) 
            => uncountables.Add(word.ToLower());

        /// <summary>
        /// Adds the plural rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="replacement">The replacement.</param>
        private static void AddPluralRule(string rule, string replacement) => plurals.Add(new InflectorRule(rule, replacement));

        /// <summary>
        /// Adds the singular rule.
        /// </summary>
        /// <param name="rule">The rule.</param>
        /// <param name="replacement">The replacement.</param>
        private static void AddSingularRule(string rule, string replacement) => singles.Add(new InflectorRule(rule, replacement));

        /// <summary>
        /// Makes the plural.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        internal static string MakePlural(this string word) => ApplyRules(plurals, word);

        /// <summary>
        /// Makes the singular.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        internal static string MakeSingular(this string word) => ApplyRules(singles, word);

        /// <summary>
        /// Applies the rules.
        /// </summary>
        /// <param name="rules">The rules.</param>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        private static string ApplyRules(IList<InflectorRule> rules, string word)
        {
            var result = word;
            if (!uncountables.Contains(word.ToLower()))
            {
                for (var i = rules.Count - 1; i >= 0; i--)
                {
                    if (rules[i].Apply(word) != null)
                    {
                        result = rules[i].Apply(word);
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Converts the string to title case.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        internal static string ToTitleCase(this string word) 
            => Regex.Replace(ToHumanCase(AddUnderscores(word)), @"\b([a-z])",
                match => match.Captures[0].Value.ToUpper());

        /// <summary>
        /// Converts the string to human case.
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">The lowercase and underscored word.</param>
        /// <returns></returns>
        internal static string ToHumanCase(this string lowercaseAndUnderscoredWord) 
            => MakeInitialCaps(Regex.Replace(lowercaseAndUnderscoredWord, @"_", " "));

        /// <summary>
        /// Convert string to proper case
        /// </summary>
        /// <param name="sourceString">The source string.</param>
        /// <returns></returns>
        internal static string ToProper(this string sourceString)
            => sourceString.ToPascalCase();

        /// <summary>
        /// Converts the string to pascal case.
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">The lowercase and underscored word.</param>
        /// <returns></returns>
        internal static string ToPascalCase(this string lowercaseAndUnderscoredWord) 
            => ToPascalCase(lowercaseAndUnderscoredWord, true);

        /// <summary>
        /// Converts text to pascal case...
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="removeUnderscores">if set to <c>true</c> [remove underscores].</param>
        /// <returns></returns>
        internal static string ToPascalCase(this string text, bool removeUnderscores)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            text = text.Replace("_", " ");
            var joinString = removeUnderscores ? string.Empty : "_";
            var words = text.Split(' ');
            if (words.Length > 1 || words[0].IsUpperCase())
            {
                for (var i = 0; i < words.Length; i++)
                {
                    if (words[i].Length > 0)
                    {
                        var word = words[i];
                        var restOfWord = word.Substring(1);

                        if (restOfWord.IsUpperCase())
                            restOfWord = restOfWord.ToLower(CultureInfo.CurrentUICulture);

                        var firstChar = char.ToUpper(word[0], CultureInfo.CurrentUICulture);
                        words[i] = string.Concat(firstChar, restOfWord);
                    }
                }
                return string.Join(joinString, words);
            }
            return string.Concat(words[0].Substring(0, 1).ToUpper(CultureInfo.CurrentUICulture), words[0].Substring(1));
        }

        /// <summary>
        /// Determines whether or not the string is upper case
        /// </summary>
        /// <param name="testString">string to test</param>
        /// <returns>true or false</returns>
        private static bool IsUpperCase(this string testString)
            => testString == testString.ToUpper();

        /// <summary>
        /// Converts the string to camel case.
        /// </summary>
        /// <param name="lowercaseAndUnderscoredWord">The lowercase and underscored word.</param>
        /// <returns></returns>
        internal static string ToCamelCase(this string lowercaseAndUnderscoredWord) 
            => MakeInitialLowerCase(ToPascalCase(lowercaseAndUnderscoredWord));

        /// <summary>
        /// Adds the underscores.
        /// </summary>
        /// <param name="pascalCasedWord">The pascal cased word.</param>
        /// <returns></returns>
        internal static string AddUnderscores(this string pascalCasedWord) 
            => Regex.Replace(Regex.Replace(Regex.Replace(pascalCasedWord, @"([A-Z]+)([A-Z][a-z])", "$1_$2"), @"([a-z\d])([A-Z])","$1_$2"), @"[-\s]", "_").ToLower();

        /// <summary>
        /// Makes the initial caps.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        internal static string MakeInitialCaps(this string word) 
            => string.Concat(word.Substring(0, 1).ToUpper(), word.Substring(1).ToLower());

        /// <summary>
        /// Makes the initial lower case.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <returns></returns>
        internal static string MakeInitialLowerCase(this string word) => string.Concat(word.Substring(0, 1).ToLower(), word.Substring(1));

        /// <summary>
        /// Adds the ordinal suffix.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        internal static string AddOrdinalSuffix(this string number)
        {
            if (number.IsNumber())
            {
                var n = int.Parse(number);
                var nMod100 = n % 100;

                if (nMod100 >= 11 && nMod100 <= 13)
                    return string.Concat(number, "th");

                switch (n % 10)
                {
                    case 1:
                        return string.Concat(number, "st");
                    case 2:
                        return string.Concat(number, "nd");
                    case 3:
                        return string.Concat(number, "rd");
                    default:
                        return string.Concat(number, "th");
                }
            }
            return number;
        }

        /// <summary>
        /// True if the string is a number, false if not
        /// </summary>
        /// <param name="aNumber">string that potentially contains a number</param>
        /// <returns>true or false</returns>
        private static bool IsNumber(this string aNumber)
            => decimal.TryParse(aNumber, out var _);

        /// <summary>
        /// Converts the underscores to dashes.
        /// </summary>
        /// <param name="underscoredWord">The underscored word.</param>
        /// <returns></returns>
        internal static string ConvertUnderscoresToDashes(this string underscoredWord) 
            => underscoredWord.Replace('_', '-');

        #region Nested type: InflectorRule

        /// <summary>
        /// Summary for the InflectorRule class
        /// </summary>
        private class InflectorRule
        {
            /// <summary>
            /// The regex pattern
            /// </summary>
            internal readonly Regex regex;

            /// <summary>
            /// The replacement text.
            /// </summary>
            internal readonly string replacement;

            /// <summary>
            /// Initializes a new instance of the <see cref="InflectorRule"/> class.
            /// </summary>
            /// <param name="regexPattern">The regex pattern.</param>
            /// <param name="replacementText">The replacement text.</param>
            internal InflectorRule(string regexPattern, string replacementText)
            {
                regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
                replacement = replacementText;
            }

            /// <summary>
            /// Applies the specified word.
            /// </summary>
            /// <param name="word">The word.</param>
            /// <returns></returns>
            internal string Apply(string word)
            {
                if (!regex.IsMatch(word))
                    return null;

                var replace = regex.Replace(word, replacement);
                if (word == word.ToUpper())
                    replace = replace.ToUpper();

                return replace;
            }
        }

        #endregion
    }
}