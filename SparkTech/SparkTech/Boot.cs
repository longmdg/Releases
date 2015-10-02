namespace SparkTech
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using LeagueSharp;

    [SuppressMessage("ReSharper", "UseNullPropagation")]
    public static class Boot
    {
        public delegate void OnInitH(EventArgs args);
        public static event OnInitH OnInit;

        static Boot()
        {
            try
            {
                switch (Game.Mode)
                {
                    case GameMode.Connecting:
                        Resources.Connecting.Instance();
                        break;
                    case GameMode.Running:
                    case GameMode.Paused:
                        Resources.Menu.Instance();
                        break;
                    case GameMode.Finished:
                        Console.WriteLine(@"Too late in the game to inject!");
                        break;
                    case GameMode.Exiting:
                        Console.WriteLine(@"[ST] - Injection requirements not met - Incorrect GameMode!");
                        break;
                    default:
                        Console.WriteLine(@"[ST] - Injection failed - Unknown GameMode");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"[ST] - Hey, I'm Spark and I'm done coding this shit :(" + Environment.NewLine + ex);
            }
        }

        internal static void FireOnInit()
        {
            if (OnInit != null)
            {
                OnInit(EventArgs.Empty);
            }
        }

        public static void Instance() { }
    }
}
