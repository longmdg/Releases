namespace SparkTech
{
    using System;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Resources;

    public static class SparkTech
    {
        public const bool CanBeLoaded = true;

        private const string ClassName = "SparkTech - Load.cs";

        private static readonly bool Subscribed;

        static SparkTech()
        {
            try
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
            catch (Exception ex)
            {
                Utility.DelayAction.Add(2500, () => Comms.Log("Failed to sign an event", ClassName, ex));
            }
        }

        private static void Init(EventArgs args)
        {
            if (Subscribed)
            {
                Utility.DelayAction.Add(50, delegate
                {
                    try
                    {
                        CustomEvents.Game.OnGameLoad -= Init;
                    }
                    catch (Exception ex)
                    {
                        Utility.DelayAction.Add(2500, () => Comms.Log("An error occured while unsubscribing", ClassName, ex));
                    }
                });
            }

            new Resources.Menu();
        }
    }
}
