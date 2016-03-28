namespace SparkTech.Helpers
{
    using System.Text.RegularExpressions;

    using LeagueSharp;

    public static class StringH
    {
        internal static readonly string LogFileEnder = "\r\n\r\n------------------------------------------------------------------------------\r\n";
       
        /// <summary>
        /// Obtains the real champion name
        /// </summary>
        /// <param name="champ">The <see cref="Obj_AI_Hero"/> instance</param>
        /// <returns></returns>
        public static string ChampionName(this Obj_AI_Hero champ)
        {
            var name = champ.ChampionName;

            switch (name.ToLower())
            {
                case "aurelionsol":
                    return "Aurelion Sol";
                case "chogath":
                    return "Cho'Gath";
                case "drmundo":
                    return "Dr. Mundo";
                case "khazix":
                    return "Kha'Zix";
                case "kogmaw":
                    return "Kog'Maw";
                case "reksai":
                    return "Rek'Sai";
                case "velkoz":
                    return "Vel'Koz";
                default:
                    return name.Space();
            }
        }

        private static ushort i;

        /// <summary>
        /// The string-spacing regular expression. This takes the presence of acronyms into account
        /// </summary>
        private static readonly Regex StringSpacer = new Regex(@"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// The string-spacing regular expression.
        /// </summary>
        private static readonly Regex StringSpacerNoAcronyms = new Regex(@"(?<!^)([A-Z])", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Spaces the input string
        /// </summary>
        /// <param name="input">The string to be spaced</param>
        /// <param name="ignoreAcronyms">If <c>true</c>, ignore acronyms</param>
        /// <returns></returns>
        public static string Space(this string input, bool ignoreAcronyms = true)
        {
            var replacement = " $" + (ignoreAcronyms ? 0 : 1);

            return (ignoreAcronyms ? StringSpacer : StringSpacerNoAcronyms).Replace(input, replacement);
        }

        /// <summary>
        /// Converts this string instance usable for the menu
        /// </summary>
        /// <param name="input">The <see cref="string"/> instance</param>
        /// <returns></returns>
        public static string ToMenuUse(this string input)
        {
            return input.Space().ToLower().Replace('\'', ' ').Trim().Replace(' ', '_');
        }

        /// <summary>
        /// Gets the menu separator text
        /// </summary>
        public static string SeparatorText => $"st_separator_{++i}";
    }
}
