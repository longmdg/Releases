namespace SparkTech
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using System;

    public class Load
    {
        private static bool subscribed;
        static Load()
        {
            Comms.Print("Static constructor of the SparkTech Load.cs called", true);

            if (Game.Mode == GameMode.Running)
            {
                Comms.Print("SparkTech - Load.cs - Fired the first library init option", true);

                try
                {
                    Summoner(new EventArgs());
                }
                catch (Exception ex)
                {
                    Comms.Print("SparkTech - Load.cs - Error: Couldn't sign an event!", true);
                    Comms.Print("Error: " + ex);
                }
            }

            else if (Game.Mode != GameMode.Running)
            {
                Comms.Print("SparkTech - Load.cs - Fired the second library init option", true);

                try
                {
                    subscribed = true;
                    CustomEvents.Game.OnGameLoad += Summoner;
                }
                catch (Exception ex)
                {
                    subscribed = false;
                    Comms.Print("SparkTech - Load.cs - Error: Couldn't sign an event!", true);
                    Comms.Print("Error: " + ex);
                }
            }
            else
            {
                Comms.Print("SparkTech - Load.cs - Error: No boot options matching!", true);
            }
        }

        private static void Summoner(EventArgs args)
        {
            STMenu.Create();

            if (subscribed)
            {
                CustomEvents.Game.OnGameLoad -= Summoner;
            }
        }
    }
}