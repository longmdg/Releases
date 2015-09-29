namespace SparkTech
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using LeagueSharp;
    using LeagueSharp.Common;

    [SuppressMessage("ReSharper", "ConvertPropertyToExpressionBody")]
    [SuppressMessage("ReSharper", "ConvertConditionalTernaryToNullCoalescing")]
    [SuppressMessage("ReSharper", "UseNullPropagation")]
    public static class SparkTech
    {
        public static bool Present = true;

        public delegate void OnMenuCreated(EventArgs args);

        public static event OnMenuCreated OnInit;

        public delegate void OnTaskFinished(EventArgs args);

        public static event OnTaskFinished OnFinalize;

        private static bool fired;

        static SparkTech()
        {
            switch (Game.Mode)
            {
                case GameMode.Running:
                case GameMode.Paused:
                    Resources.Menu.Create();
                    break;
                case GameMode.Connecting:
                    Resources.LoadingScreen.Init();
                    CustomEvents.Game.OnGameLoad += Init;
                    break;
                case GameMode.Finished:
                    Game.PrintChat("[ST] " + ObjectManager.Player.ChampionName + " - too late in the game to inject!");
                    break;
                case GameMode.Exiting:
                    Console.WriteLine(@"SparkTech - Injection requirements not met - Incorrect GameMode!");
                    break;
                default:
                    Console.WriteLine(@"SparkTech - Injection requirements not met - Unknown GameMode");
                    break;
            }
        }

        private static void Init(EventArgs args)
        {
            CustomEvents.Game.OnGameLoad -= Init;
            Resources.Menu.Create();
        }

        internal static void FireOnInit()
        {
            if (OnInit != null)
            {
                OnInit(new EventArgs());
            }
        }

        public static void FireOnFinalized()
        {
            if (fired)
            {
                return;
            }

            fired = true;

            Utility.DelayAction.Add(
                250,
                () =>
                    {
                        if (OnFinalize != null)
                        {
                            OnFinalize(new EventArgs());
                        }
                    });
        }
    }
}