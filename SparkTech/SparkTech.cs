namespace SparkTech
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using LeagueSharp;
    using LeagueSharp.Common;

    [SuppressMessage("ReSharper", "ConvertPropertyToExpressionBody")]
    [SuppressMessage("ReSharper", "UseNullPropagation")]
    public sealed class SparkTech
    {
        public delegate void OnMenuCreated(EventArgs args);
        public static event OnMenuCreated OnInit;

        private static SparkTech inst;
        public static SparkTech Instance
        {
            get
            {
                return inst ?? (inst = new SparkTech());
            }
        }

        private SparkTech()
        {
            try
            {
                switch (Game.Mode)
                {
                    case GameMode.Running:
                    case GameMode.Paused:
                        Resources.Menu.Create();
                        break;
                    case GameMode.Connecting:
                        CustomEvents.Game.OnGameLoad += Inject;
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
            catch (Exception ex)
            {
                Console.WriteLine(@"SparkTech - An unknown error occured during the injection:"  + Environment.NewLine + ex);
            }
        }

        internal static void FireOnInit()
        {
            if (OnInit != null)
            {
                OnInit(new EventArgs());
            }
        }
        
        private static void Inject(EventArgs args)
        {
            Resources.Menu.Create();
            GC.Collect();
        }

        ~SparkTech()
        {
            CustomEvents.Game.OnGameLoad -= Inject;
        }
    }
}