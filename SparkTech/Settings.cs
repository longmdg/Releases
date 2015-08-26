using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace SparkTech
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    
    //reenable when L# supports .NET 4.6
    [SuppressMessage("ReSharper", "ConvertPropertyToExpressionBody")]

    public static class Settings
    {
        #region MenuBools

        internal static bool Debug = false;

        public static bool SkipNoUpdate = true;
        public static bool UpdateCheck = true;
        public static int UpdateCheckDelay = 250;

        private static float _spaghettiLimiter;
        private static bool _zoomhackEnabled;
        private static bool ZoomHackTurnedOn
        {
            get
            {
                return LibraryMenu.Item("zoomhack").GetValue<bool>();
            }
        }

        #endregion

        public static Menu LibraryMenu;

        internal static void LoadStuff()
        {
            LibraryMenu = new Menu("[ST] Core", "SparkTech", true);

            var F5Settings = new Menu("F5Settings", "F5Settings");
            {
                F5Settings.AddItem(new MenuItem("defaultsettings", "Use Default Settings")).SetValue(true);

            }
            LibraryMenu.AddSubMenu(F5Settings);

            var BanMenu = new Menu("Ban me please", "BanMenu");
            {
                BanMenu.AddItem((new MenuItem("zoomhack", "Use ZoomHack (bannable!)")).SetValue(false));
            }
            LibraryMenu.AddSubMenu(BanMenu);

            LibraryMenu.AddItem(new MenuItem("onupdatedelay", "Delay in checking for menu changes")).SetValue(new Slider(8, 0, 1000));


            // Comms.Print("Loaded.");

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

            LibraryMenu.AddToMainMenu();

            _zoomhackEnabled = false;
            Game.OnUpdate += SettingsUpdate;

        }

        private static void SettingsUpdate(EventArgs args)
        {
            if (Environment.TickCount - _spaghettiLimiter < 80)
            {
                return;
            }

            _spaghettiLimiter = Environment.TickCount;

            if (ZoomHackTurnedOn && !_zoomhackEnabled)
            {
                _zoomhackEnabled = true;
                Hacks.ZoomHack = true;
            }
            else if (!ZoomHackTurnedOn && _zoomhackEnabled)
            {
                _zoomhackEnabled = false;
                Hacks.ZoomHack = false;
            }
        }
    }
}
