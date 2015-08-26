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
    // [SuppressMessage("ReSharper", "ConvertPropertyToExpressionBody")]

    public class Settings
    {
        #region MenuBools

        internal static bool Debug = false;

        public static bool SkipNoUpdate = true;
        public static bool UpdateCheck = true;
        public static int UpdateCheckDelay = 250;

        private static float _spaghettiLimiter;
        // private static bool _zoomhackEnabled;
        // private static bool _dciEnabled;
        // private static bool ZoomHackTurnedOn { get { return LibraryMenu.Item("zoomhack").GetValue<bool>(); } }
        // private static bool DCITurnedOn { get { return LibraryMenu.Item("dcindicator").GetValue<bool>(); } }

        #endregion

        public static Menu LibraryMenu;

        internal static void LoadStuff()
        {
            LibraryMenu = new Menu("[ST] Core", "SparkTech", true);

            var F5Settings = new Menu("F5Settings", "F5Settings");
            {

                F5Settings.AddItem(new MenuItem("defaultsettings", "Use Recommended Settings")).SetValue(true);
                F5Settings.AddItem((new MenuItem("", "For [ST] assemblies only.")));

            }
            LibraryMenu.AddSubMenu(F5Settings);

            var BanMenu = new Menu("I want to get banned", "BanMenu");
            {
                var DCIndicator =
                    BanMenu.AddItem((new MenuItem("dcindicator", "Disable Cast Indicator")).SetValue(false));
                DCIndicator.SetValue(Hacks.DisableCastIndicator);
                DCIndicator.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs args)
                    {
                        Hacks.DisableCastIndicator = args.GetNewValue<bool>();
                    };

                BanMenu.AddItem((new MenuItem("", "")));

                var ZoomHack = BanMenu.AddItem((new MenuItem("zoomhack", "Use ZoomHack (bannable!)")).SetValue(false));
                ZoomHack.SetValue(Hacks.ZoomHack);
                ZoomHack.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs args)
                    {
                        Hacks.ZoomHack = args.GetNewValue<bool>();
                    };

                BanMenu.AddItem((new MenuItem("confirm", "Care: ZoomHack can get you banned!")));
            }

            LibraryMenu.AddSubMenu(BanMenu);

            // LibraryMenu.AddItem(new MenuItem("onupdatedelay", "Delay in checking for menu changes")).SetValue(new Slider(300, 0, 1000));


            // Comms.Print("Loaded!");

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

            // _zoomhackEnabled = false;

            Utility.DelayAction.Add(1500, () =>
            {
                LibraryMenu.AddToMainMenu();
                Game.OnUpdate += OnSettingsChange;
            });

        }

        

        private static void OnSettingsChange(EventArgs args)
        {
            if (Environment.TickCount - _spaghettiLimiter < LibraryMenu.Item("onupdatedelay").GetValue<Slider>().Value)
            {
                return;
            }

            _spaghettiLimiter = Environment.TickCount;

            /*

            if (!_zoomhackEnabled && ZoomHackTurnedOn && LibraryMenu.Item("confirm").GetValue<bool>())
            {
                _zoomhackEnabled = true;
                Hacks.ZoomHack = true;
            }
            if (_zoomhackEnabled && (!ZoomHackTurnedOn || !LibraryMenu.Item("confirm").GetValue<bool>()))
            {
                _zoomhackEnabled = false;
                Hacks.ZoomHack = false;
            }
            if (DCITurnedOn && !_dciEnabled)
            {
                _dciEnabled = true;
                Hacks.DisableCastIndicator = true;
            }
            if (!DCITurnedOn && _dciEnabled)
            {
                _dciEnabled = false;
                Hacks.DisableCastIndicator = false;
            }

            */

            Console.WriteLine("");

         // Comms.Print("");

        } 
        
        /*
        
        Code
        
        */
    }
}
