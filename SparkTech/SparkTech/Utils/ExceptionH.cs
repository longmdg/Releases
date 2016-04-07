namespace SparkTech.Utils
{
    using System;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.UI.IMenu;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;

    using SparkTech.Executors;

    /// <summary>
    /// The helper class for <see cref="Exception"/> and logging
    /// </summary>
    public class ExceptionH : IMenuPiece
    {
        /// <summary>
        /// Initializes static members of the <see cref="ExceptionH"/> class
        /// </summary>
        static ExceptionH()
        {
            Translations.RegisterReplacement("exception_count", () => count.ToString());

            Game.OnEnd += delegate
                {
                    if (menu?["st_core_logging_onend"].GetValue<MenuBool>().Value != true)
                    {
                        return;
                    }

                    var text = $"Exception count: {count}";

                    if (count > 0 && menu["st_core_logging_file"].GetValue<MenuBool>().Value)
                    {
                        text += " See the logs for more details";
                    }

                    Comms.Print(text);
                };
        }

        /// <summary>
        /// The cached <see cref="E:Write"/> delegate
        /// </summary>
        private static readonly Logging.WriteDelegate WriteT = Logging.Write(true), WriteF = Logging.Write();

        /// <summary>
        /// The current error amount
        /// </summary>
        private static int count;

        /// <summary>
        /// Logs an exception
        /// </summary>
        /// <param name="ex">The exception to be logged</param>
        /// <param name="level">The severity level</param>
        internal static void Catch(Exception ex, LogLevel level)
        {
            // TODO: Something smarter, perhaps with casting the ex to dynamic?
            count++;
            (menu?["st_core_logging_file"].GetValue<MenuBool>().Value == false ? WriteF : WriteT)(level, ex);
        }

        /// <summary>
        /// The backing field for <see cref="E:Piece"/>
        /// </summary>
        private static Menu menu;

        /// <summary>
        /// The submenu to be added to the root
        /// </summary>
        public Menu Piece
        {
            get
            {
                // ReSharper disable once InvertIf
                if (menu == null)
                {
                    menu = new Menu("st_core_logging", "Error logging");
                    {
                        menu.Add(new MenuBool("st_core_logging_file", "Log to file", true));
                        menu.Add(new MenuBool("st_core_logging_onend", "Display info when game ends", true));
                        menu.Add(new MenuSeparator("st_core_logging_info", "Current exception count: 0"));
                    }
                }

                return menu;
            }
        }
    }
}