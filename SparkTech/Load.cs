namespace SparkTech
{
    using LeagueSharp;
    using LeagueSharp.Common;
    using System;
    using System.Net;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;

    public static class Load
    {
        private static bool summoned;

        static Load()
        {
            Comms.Print("Static constructor of the SparkTech Load.cs called", true);
            summoned = false;
        }

        public static void Library()
        {
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
                    Comms.Print("Error: "+ ex);
                }
            }
            else if (Game.Mode != GameMode.Running)
            {
                Comms.Print("SparkTech - Load.cs - Fired the second library init option", true);
                try
                {
                    CustomEvents.Game.OnGameLoad += Summoner;
                }
                catch (Exception ex)
                {
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
            if (!summoned)
            {
                summoned = true;
                Settings.Fire();
                return;
            }
            Comms.Print("Error: Library already loaded!", true);
        }

        #region LibraryUpdateCheck

        

        #endregion LibraryUpdateCheck

    }
}