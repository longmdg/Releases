namespace SparkTech
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using LeagueSharp;
    using LeagueSharp.Common;

    [SuppressMessage("ReSharper", "ConvertPropertyToExpressionBody")]
    [SuppressMessage("ReSharper", "LocalizableElement")]
    public sealed class SparkTech
    {
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
                        new Resources.Menu();
                        break;
                    case GameMode.Connecting:
                        CustomEvents.Game.OnGameLoad += Init;
                        break;
                    case GameMode.Finished:
                        Game.PrintChat("[ST] " + ObjectManager.Player.ChampionName + " - too late in the game to inject!");
                        break;
                    case GameMode.Exiting:
                        Console.WriteLine("SparkTech - Injection requirements not met - Incorrect GameMode!");
                        break;
                    default:
                        Console.WriteLine("SparkTech - Injection requirements not met - Unknown GameMode");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SparkTech - An unknown error occured during the injection:\n{0}", ex);
            }
        }

        private static void Init(EventArgs args)
        {
            new Resources.Menu();
            GC.Collect();  // TODO: Drink devs' tears after they realise how awful this line is :^)
        }

        ~SparkTech()
        {
            CustomEvents.Game.OnGameLoad -= Init;
        }
    }
}