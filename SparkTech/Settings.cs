//using System.Linq;

using LeagueSharp.Common;
using System.Diagnostics.CodeAnalysis;

//using SharpDX;

namespace SparkTech
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "ConvertPropertyToExpressionBody")] // reenable when L# supports .NET 4.6
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public class Settings
    {
        #region MenuBools

        public static bool MenuLoaded;
        internal static bool Debug = true;

        public static bool SkipNoUpdate = false;
        public static bool UpdateCheck = true;
        public static int UpdateCheckDelay = 250;

        private static float spaghettiLimiter;

        private static bool LoadHack
        {
            get { return LibraryMenu.Item("hack").GetValue<bool>(); }
        }

        #endregion MenuBools

        public static Menu LibraryMenu;

        internal static void LoadStuff()
        {
            MenuLoaded = true;

            LibraryMenu = new Menu("[ST] Core", "SparkTech", true);

            Menu F5Settings = new Menu("F5Settings", "F5Settings");
            {
                F5Settings.AddItem(new MenuItem("defaultsettings", "Use Recommended Settings")).SetValue(true);
                F5Settings.AddItem((new MenuItem("1", "For [ST] assemblies only.")));
                F5Settings.AddItem((new MenuItem("hack", "Load ZoomHack"))).SetValue(false);
            }
            LibraryMenu.AddSubMenu(F5Settings);

            new Test();

            if (LoadHack)
            {
                new Hacks.ZoomHack();
            }

            LibraryMenu.AddItem(new MenuItem("onupdatedelay", "Delay in checking for menu changes")).SetValue(new Slider(300, 0, 1000));

            Utility.DelayAction.Add(1500, () =>
            {
                if (Hacks.HackInited)
                {
                    LibraryMenu.AddSubMenu(Hacks.HacksMenu);
                }

                LibraryMenu.AddToMainMenu();
                //Game.OnUpdate += OnSettingsChange;
            });
        }

        /*
                private static void OnSettingsChange(EventArgs args)
                {
                    if (Environment.TickCount - _spaghettiLimiter < LibraryMenu.Item("onupdatedelay").GetValue<Slider>().Value)
                    {
                        return;
                    }

                    _spaghettiLimiter = Environment.TickCount;

                    Console.WriteLine("");
                }
        */

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