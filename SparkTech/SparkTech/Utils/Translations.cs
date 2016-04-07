namespace SparkTech.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    using LeagueSharp.SDK.Core.UI.IMenu;
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    using SparkTech.Cache;
    using SparkTech.Enumerations;
    using SparkTech.Executors;
    using SparkTech.Properties;

    /// <summary>
    /// Manages the translation data
    /// </summary>
    internal class Translations : IMenuPiece
    {
        /// <summary>
        /// Points to a replacement
        /// </summary>
        /// <returns></returns>
        internal delegate string TextPointer();

        /// <summary>
        /// Contains the cached <see cref="CultureInfo"/> instances for every <see cref="Language"/>
        /// </summary>
        private static readonly Dictionary<Language, CultureInfo> LanguageInfos;

        /// <summary>
        /// Initializes static members of the <see cref="Translations"/> class
        /// </summary>
        static Translations()
        {
            LanguageInfos = EnumCache<Language>.Values.ToDictionary(
                language => language,
                language => new CultureInfo(EnumCache<Language>.Description(language)));
        }

        /// <summary>
        /// Gets the currently used language if it's supported, otherwise <c><see cref="Language.English"/></c>
        /// </summary>
        internal static Language CurrentLanguage => (from pair in LanguageInfos where pair.Value.Equals(CultureInfo.CurrentUICulture) select pair.Key).SingleOrDefault();

        /// <summary>
        /// Gets a raw translation from the resource files using the key and an optional language
        /// </summary>
        /// <param name="name"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        internal static string GetTranslation(string name, Language? language)
        {
            return language.HasValue
                       ? Resources.ResourceManager.GetString(name, LanguageInfos[language.Value])
                       : Resources.ResourceManager.GetString(name);
        }

        /// <summary>
        /// Contains the pointers to current values of the keys
        /// </summary>
        private static readonly Dictionary<string, TextPointer> Replacements = new Dictionary<string, TextPointer>();

        /// <summary>
        /// Registers a new replacement
        /// </summary>
        /// <param name="key">The key to be seeked for</param>
        /// <param name="replacement">The string to replace the key when matched</param>
        internal static void RegisterReplacement(string key, TextPointer replacement) => Replacements.Add(key, replacement);

        /// <summary>
        /// The regular expression that will match all the substrings surrounded by braces
        /// </summary>
        private static readonly Regex BracesAroundTextRegex = new Regex(@"{\w+}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// The regular expression that will find all the braces in a specified string
        /// </summary>
        private static readonly Regex BraceFinderRegex = new Regex("[{}]", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        /// <summary>
        /// Updates the display name of a specified menu component to match the new data
        /// </summary>
        /// <param name="component">The <see cref="AMenuComponent"/> instance to be updated</param>
        /// <param name="language">The specified language</param>
        /// <returns></returns>
        internal static void Update(AMenuComponent component, Language? language = null)
        {
            var name = component.Name;

            try
            {
                var translation = GetTranslation(name, language);

                component.DisplayName = (from Match match in BracesAroundTextRegex.Matches(translation)
                                         select match.Groups).SelectMany(
                                             groups =>
                                             {
                                                 var list = new List<string>(groups.Count);

                                                 for (int i = 0; i < groups.Count; i++)
                                                 {
                                                     list.Add(groups[i].Value);
                                                 }

                                                 return list;
                                             })
                    .Aggregate(
                        translation,
                        (current, match) =>
                        current.Replace(match, Replacements[BraceFinderRegex.Replace(match, string.Empty)]()));
            }
            catch (KeyNotFoundException ex)
            {
                ex.Catch($"Couldn't translate {name}");
            }
            catch (Exception ex)
            {
                ex.Catch();
            }
        }

        /// <summary>
        /// Updates all of the menu components
        /// </summary>
        internal static void UpdateAll()
        {
            foreach (var component in Core.Menu.Components.Values.Where(component => !component.Name.EndsWith("nt")))
            {
                Update(component);
            }
        }

        /// <summary>
        /// The submenu to be added to the root
        /// </summary>
        Menu IMenuPiece.Piece
        {
            get
            {
                var menu = new Menu("st_core_translations", string.Empty);
                {
                    var language = menu.Add(new MenuList<Language>("st_core_translations_selected", string.Empty, EnumCache<Language>.Values));

                    if (Core.FirstRun)
                    {
                        language.SelectedValue = CurrentLanguage;
                    }

                    language.ValueChanged += delegate
                    {
                        var value = LanguageInfos[language.SelectedValue];

                        if (CultureInfo.CurrentUICulture.Equals(value))
                        {
                            return;
                        }

                        CultureInfo.CurrentUICulture = value;

                        UpdateAll();
                    };
                }

                return menu;
            }
        }
    }
}