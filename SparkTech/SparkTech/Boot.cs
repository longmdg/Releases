namespace SparkTech
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using LeagueSharp;
    using LeagueSharp.Common;

    [SuppressMessage("ReSharper", "UseNullPropagation")]

    public static class Boot
    {
        static Boot()
        {
            SwitchGameMode();
        }

        public delegate void OnInitDelegate(EventArgs args);

        public static event OnInitDelegate OnInit;

        internal static void FireOnInit()
        {
            if (OnInit != null)
            {
                OnInit(EventArgs.Empty);
            }
        }

        public static void Initialize() { }

        private static void SwitchGameMode()
        {
            switch (Game.Mode)
            {
                case GameMode.Connecting:
                    Resources.Connecting.Instance();
                    break;
                case GameMode.Running:
                case GameMode.Paused:
                    Resources.MenuST.Instance();
                    break;
                case GameMode.Finished:
                    Console.WriteLine(@"Too late in the game to inject!");
                    break;
                case GameMode.Exiting:
                    Console.WriteLine(@"ST - Injection requirements not met - Incorrect GameMode!");
                    break;
                default:
                    Console.WriteLine(@"ST - An unknown injection failure occured, retrying...");
                    Utility.DelayAction.Add(125, SwitchGameMode);
                    break;
            }
        }
    }
}
