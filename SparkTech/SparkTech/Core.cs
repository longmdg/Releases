namespace SparkTech
{
    using System;
    using System.Reflection;

    using LeagueSharp;
    using LeagueSharp.SDK;
    using LeagueSharp.SDK.Core.UI.IMenu;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;
    using LeagueSharp.SDK.Core.Utils;

    using SparkTech.Base;
    using SparkTech.Enumerations;
    using SparkTech.EventData;
    using SparkTech.Executors;

    using static System.String;

    using Orbwalker = SparkTech.Enumerations.Orbwalker;
    using SDKOrbwalker = LeagueSharp.SDK.Orbwalker;

    /// <summary>
    /// Contains the key components of the library
    /// </summary>
    public static class Core
    {
        /// <summary>
        /// The root menu instance
        /// </summary>
        internal static readonly Menu Menu;

        /// <summary>
        /// The currently executing <see cref="System.Reflection.Assembly"/> instance
        /// </summary>
        internal static readonly Assembly Assembly;

        /// <summary>
        /// Represents the currently used <see cref="Orbwalker"/>
        /// </summary>
        internal static Orbwalker UsedOrbwalker;

        #region API

        /// <summary>
        /// The player
        /// </summary>
        public static readonly Obj_AI_Hero Player;

        /// <summary>
        /// The champion name
        /// </summary>
        public static readonly string ChampionName;
        
        /// <summary>
        /// Returns a value indicating whether this run is a first one in this environment.
        /// </summary>
        public static readonly bool FirstRun;

        /// <summary>
        /// The hero instance
        /// </summary>
        public static HeroBase Champion { get; internal set; }

        /// <summary>
        /// The common orbwalker instance
        /// </summary>
        // ReSharper disable once InconsistentNaming
        public static SDKOrbwalker SDKOrbwalker => Variables.Orbwalker;

        /// <summary>
        /// Fired when the game engine updates. Contains custom event data.
        /// </summary>
        public static event EventDataHandler<GameUpdateEventArgs> GameUpdate;

        #endregion
        
        #region Creator

        static Core()
        {
            try
            {
                Game.OnUpdate += delegate
                {
                    Mode currentMode;

                    switch (UsedOrbwalker)
                    {
                        case Orbwalker.SparkWalker:
                            currentMode = SparkWalker.Orbwalker.Mode;
                            break;
                        case Orbwalker.SDKOrbwalker:
                            switch (SDKOrbwalker.GetActiveMode())
                            {
                                case OrbwalkingMode.LastHit:
                                    currentMode = Mode.LastHit;
                                    break;
                                case OrbwalkingMode.Hybrid:
                                    currentMode = Mode.Harass;
                                    break;
                                case OrbwalkingMode.LaneClear:
                                    currentMode = Mode.LaneClear;
                                    break;
                                case OrbwalkingMode.Combo:
                                    currentMode = Mode.Combo;
                                    break;
                                case OrbwalkingMode.None:
                                    currentMode = Mode.None;
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException(nameof(SDKOrbwalker.GetActiveMode), SDKOrbwalker.GetActiveMode(), null);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    GameUpdate?.Invoke(new GameUpdateEventArgs(UsedOrbwalker, currentMode));
                };

                Assembly = Assembly.GetExecutingAssembly();

                Player = ObjectManager.Player;

                ChampionName = Player.ChampionName;

                Menu = new Menu("st_core", Empty, true).Attach();
                {
                    var frun = Menu.Add(new MenuBool("st_first_run_nt", Empty, true));
                    FirstRun = frun.Value;
                    frun.Value = frun.Visible = false;
                }

                var baseMenu = Menu.Add(new Menu("st_base", ""));
                {

                }

                var commsMenu = Menu.Add(new Menu("st_comms", ""));
                {

                }

                var lang = Menu.Add(new MenuList<Language>("st_language", ""));

                lang.ValueChanged += delegate
                        {
                            DelayAction.Add(1, delegate
                                    {
                                        ActiveLanguage = lang.SelectedValue;
                                        LanguageData.Rename(Menu);
                                    });
                        };

                if (FirstRun)
                {
                    lang.SelectedValue = LanguageData.LoaderLanguage();
                }
            }
            catch (Exception ex)
            {
                Logging.Write()(LogLevel.Error, ex);
            }
        }

        #endregion
    }
}