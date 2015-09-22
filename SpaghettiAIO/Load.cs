namespace SparkTech
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Resources;

    internal static class Load
    {
        private const string CName = "SparkTech - Load.cs";
        private static readonly bool Subscribed;

        static Load()
        {
            if (Game.Mode == GameMode.Running)
            {
                Subscribed = false;
                Init(new EventArgs());
            }
            else
            {
                Subscribed = true;
                CustomEvents.Game.OnGameLoad += Init;
            }
        }

        static void Main()
        { }

        private static void Init(EventArgs args)
        {
            if (Subscribed)
            {
                try
                {
                    CustomEvents.Game.OnGameLoad -= Init;
                }
                catch (Exception ex)
                {
                    var huh = delegate() { Comms.Log("An error occured while unsubscribing", CName, ex); };
                    Resources.Menu.OnMenuCreated += huh;
                }
            }
            // ReSharper disable once ObjectCreationAsStatement
            new Resources.Menu();
        }
    }
}
