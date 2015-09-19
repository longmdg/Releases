namespace SparkTech
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    public class Load
    {
        private const string ClassName = "SparkTech - Load.cs";
        private static readonly bool Subbed;
        static Load()
        {
            Comms.Print("Static constructor of the - " + ClassName + " - called", true);
            try
            {
                if (Game.Mode == GameMode.Running)
                {
                    Comms.Print(ClassName + " - Fired the first library init option", true);
                    Subbed = false;
                    Summoner(new EventArgs());
                }
                else
                {
                    Comms.Print(ClassName + " - Fired the second library init option", true);
                    Subbed = true;
                    CustomEvents.Game.OnGameLoad += Summoner;
                }
            }
            catch (Exception ex)
            {
                Comms.Print(ClassName + " - Error: Couldn't sign an event!", true);
                Comms.Print(ex.ToString(), true);
            }
        }

        private static void Summoner(EventArgs args)
        {
            if (Subbed)
            {
                CustomEvents.Game.OnGameLoad -= Summoner;
            }
            // ReSharper disable once UnusedVariable
            var menu = new STMenu();
        }
    }
}
