using System;
using System.Diagnostics.CodeAnalysis;
//using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;
//using SharpDX;

namespace SparkTech
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]

    //reenable when L# supports .NET 4.6
    // [SuppressMessage("ReSharper", "ConvertPropertyToExpressionBody")]

    public class Settings
    {
        #region MenuBools

        public static bool MenuLoaded;
        internal static bool Debug = false;

        public static bool SkipNoUpdate = true;
        public static bool UpdateCheck = true;
        public static int UpdateCheckDelay = 250;

        private static float _spaghettiLimiter;

        private static bool LoadHack
        {
            get { return LibraryMenu.Item("hack").GetValue<bool>(); }
        }

        #endregion

        public static Menu LibraryMenu;

        internal static void LoadStuff()
        {
            MenuLoaded = true;

            LibraryMenu = new Menu("[ST] Core", "SparkTech", true);

            var F5Settings = new Menu("F5Settings", "F5Settings");
            {
                F5Settings.AddItem(new MenuItem("defaultsettings", "Use Recommended Settings")).SetValue(true);
                F5Settings.AddItem((new MenuItem("1", "For [ST] assemblies only.")));
                F5Settings.AddItem((new MenuItem("hack", "For [ST] assemblies only."))).SetValue(false);
            }
            LibraryMenu.AddSubMenu(F5Settings);

            if (LoadHack)
            {
                // ReSharper disable once ObjectCreationAsStatement
                new Hack();
            }

            LibraryMenu.AddItem(new MenuItem("onupdatedelay", "Delay in checking for menu changes")).SetValue(new Slider(300, 0, 1000));

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

            Console.WriteLine("");

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
