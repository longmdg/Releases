namespace SparkTech
{
    using System;
    using LeagueSharp;
    using LeagueSharp.Common;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "ConvertPropertyToExpressionBody")] // reenable when L# supports .NET 4.6
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public static class Settings
    {
        #region MenuBools

        public static bool MenuLoaded;
        internal static bool Debug = true;

        public static bool SkipNoUpdate = false;
        public static bool UpdateCheck = true;
        public static int UpdateCheckDelay = 250;

        private static float spaghettiLimiter;


        internal static bool LoadZoomHack
        {
            get { return LibraryMenu.Item("hack").GetValue<bool>(); }
        }

        #endregion MenuBools

        public static Menu LibraryMenu;

        internal static void Fire()
        {
            MenuLoaded = true;

            LibraryMenu = new Menu("[ST] Core", "SparkTech", true);

            Menu F5Settings = new Menu("F5Settings", "F5Settings");
            {
                F5Settings.AddItem((new MenuItem("hack", "Load ZoomHack"))).SetValue(false);
            }
            LibraryMenu.AddSubMenu(F5Settings);

            Menu orbmenu = new Menu("Orb", "orb1");
            Orbwalker.Init(orbmenu);
            LibraryMenu.AddSubMenu(orbmenu);

            if (Drawing.Height == 1920 && Drawing.Width == 1080)
            {
                new Extensions();
            }
            

            if (LibraryMenu.Item("hack").GetValue<bool>())
            {
                new Cheats();
            }

            LibraryMenu.AddItem(new MenuItem("onupdatedelay", "Delay in checking for menu changes")).SetValue(new Slider(300, 0, 1000));

            Utility.DelayAction.Add(150, () =>
            {
                LibraryMenu.AddToMainMenu();
                Game.OnUpdate += OnSettingsChange;
            });
        }
        
                private static void OnSettingsChange(EventArgs args)
                {
                    if (Environment.TickCount - spaghettiLimiter < LibraryMenu.Item("onupdatedelay").GetValue<Slider>().Value)
                    {
                        return;
                    }

                    spaghettiLimiter = Environment.TickCount;

                    /*
            
            
                    */
                }
        

        /*

F5Settings -> Default Settings?
F5Settings -> Subsciption Manager
F5Settings -> Target Selector
F5Settings -> Orbwalker
F5Settings -> Prediction

Messaging -> Message output (PrintChat, Notifs, Console)
Messaging -> Delay / Time

Updater -> Check for updates
Updater -> Display the message only if update found
Updater -> Update delay

more 2 come

*/

        /*

        Code

        */
    }
}