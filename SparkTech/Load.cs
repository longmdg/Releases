namespace SparkTech
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using System;

    public class Load
    {
        private const string ClassName = "SparkTech - Load.cs - ";
        private static readonly bool Subscribed;

        static Load()
        {
            Comms.Print("Static constructor of the - " + ClassName + " called", true);
            try
            {
                if (Game.Mode == GameMode.Running)
                {
                    Comms.Print(ClassName + "Fired the first library init option", true);
                    Summoner(new EventArgs());
                }
                else
                {
                    Comms.Print(ClassName + "Fired the second library init option", true);
                    Subscribed = true;
                    CustomEvents.Game.OnGameLoad += Summoner;
                }
            }
            catch (Exception ex)
            {
                Comms.Print(ClassName + "Error: Couldn't sign an event!", true);
                Comms.Print("Error: " + ex, true);
            }
        }

        private static void Summoner(EventArgs args)
        {
            STMenu.Create();
            if (Subscribed)
            {
                CustomEvents.Game.OnGameLoad -= Summoner;
            }
        }
    }
}