namespace SparkTech
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics.CodeAnalysis;

    using LeagueSharp;
    using LeagueSharp.Common;

    using Resources.Base;

    // Separator - <menuname>.AddItem((new MenuItem("st_separator_" + Helper.NextNumber, "")));

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    [SuppressMessage("ReSharper", "ConvertPropertyToExpressionBody")]
    public class STMenu
    {
        public static Menu LibraryMenu;

        static STMenu()
        {
            try
            {
                LibraryMenu = new Menu("[ST] Core Features", "SparkTech", true);

                #region F5Settings

                /*

                    var F5Settings = new Menu("F5Settings", "F5Settings");
                    {
                        var st_f5settings_zoomhack = F5Settings.AddItem((new MenuItem("st_f5settings_zoomhack", "Load ZoomHack (unsafe!)"))).SetValue(false);
                        st_f5settings_zoomhack.ValueChanged += (s, a) => Settings.LoadZoomHack = a.GetNewValue<bool>(); // redundant?
                    }
                    LibraryMenu.AddSubMenu(F5Settings);
                
                */

                #endregion

                #region Orbwalking

                
                    var orbmenu = new Menu("BrianWalker", "lelelelell");
                    BrianWalker.Init(orbmenu);
                    LibraryMenu.AddSubMenu(orbmenu);
                

                #endregion

                new Extensions();

                #region Cheats
                /*
                Menu cheatsMenu = new Menu("Cheats", "st_cheats_head");
                Settings.LoadZoomHack = LibraryMenu.Item("st_f5settings_zoomhack").GetValue<bool>(); // ???

                if (Settings.LoadZoomHack)
                {
                    var zoomHack =
                        cheatsMenu.AddItem(new MenuItem("st_cheats_zoomhack_enable", "Enable ZoomHack").SetValue(false));
                    zoomHack.ValueChanged += (s, a) =>
                        {
                            Settings.ChValue1 = a.GetNewValue<bool>();
                            Utility.DelayAction.Add(10, () => Hacks.ZoomHack = Settings.ChValue1 && Settings.ChValue2);
                        };

                    cheatsMenu.AddItem((new MenuItem("st_separator_" + NextNumber, "")));

                    var confirmation =
                        cheatsMenu.AddItem(
                            new MenuItem("st_cheats_zoomhack_confirmation", "Confirm your selection").SetValue(false));
                    confirmation.SetValue(Hacks.ZoomHack);
                    confirmation.ValueChanged += (s, a) =>
                        {
                            Settings.ChValue2 = a.GetNewValue<bool>();
                            Utility.DelayAction.Add(10, () => Hacks.ZoomHack = Settings.ChValue1 && Settings.ChValue2);
                        };

                    cheatsMenu.AddItem(
                        new MenuItem("st_separator_" + NextNumber, "Warning: ZoomHack is extremely unsafe!"));
                    cheatsMenu.AddItem(new MenuItem("st_separator_" + NextNumber, ""));

                    var dcIndicator =
                        cheatsMenu.AddItem(new MenuItem("st_cheats_dci", "Disable Cast Indicator").SetValue(false));
                    dcIndicator.SetValue(Hacks.DisableCastIndicator);
                    dcIndicator.ValueChanged += (s, a) => Hacks.DisableCastIndicator = a.GetNewValue<bool>();

                    cheatsMenu.AddItem(
                        new MenuItem("st_separator_" + NextNumber, "It disables the display of your skill ranges"));

                    LibraryMenu.AddSubMenu(cheatsMenu);
                }
                */
                #endregion

                
                LibraryMenu.AddItem(new MenuItem("onupdatedelay", "Delay in checking for menu changes")).SetValue(new Slider(300, 0, 1000));

                LibraryMenu.AddToMainMenu();

                #region Bools

                // Settings.ChValue1 = LibraryMenu.Item("st_cheats_zoomhack_enable").GetValue<bool>();
                // Settings.ChValue2 = LibraryMenu.Item("st_cheats_zoomhack_confirmation").GetValue<bool>();

                #endregion

                // Updater.Check("SparkTech");
                // Game.OnUpdate += Settings.OnSettingsChange;
            }
            catch (Exception e)
            {
                Comms.Print("An error occured while injecting. Press F5 to retry.");
                Comms.Print("Menu not built!", true);
                Comms.Print(e.ToString(), true);
            }
        }

        #region Misc

        private static int tempNumber;
        public static int NextNumber
        {
            get
            {
                return tempNumber++;
            }
        }

        #endregion
    }
}