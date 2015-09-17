namespace SparkTech
{
    using LeagueSharp;
    using LeagueSharp.Common;

    internal class Cheats
    {
        private static readonly Menu CMenu;

        static Cheats()
        {
            CMenu = new Menu("Cheats", "st_cheats_mainmenu");

            var zoomHack = CMenu.AddItem((new MenuItem("st_cheats_zoomhack_enable", "Enable ZoomHack")).SetValue(false));
            zoomHack.ValueChanged += (sender, args) =>
            {
                if (CMenu.Item("st_cheats_zoomhack_confirmation").GetValue<bool>())
                {
                    Hacks.ZoomHack = args.GetNewValue<bool>();
                }
            };

            CMenu.AddItem((new MenuItem("st_cheats_separator_1", "")));

            var confirmation = CMenu.AddItem((new MenuItem("st_cheats_zoomhack_confirmation", "Confirm your selection")).SetValue(false));
            confirmation.SetValue(Hacks.ZoomHack);
            confirmation.ValueChanged += (sender, args) =>
                {
                    if (CMenu.Item("st_cheats_zoomhack_enable").GetValue<bool>())
                    {
                        Hacks.ZoomHack = args.GetNewValue<bool>();
                    }
                };

            CMenu.AddItem((new MenuItem("st_cheats_separator_2", "Warning: ZoomHack is extremely unsafe!")));
            CMenu.AddItem((new MenuItem("st_cheats_separator_3", "")));

            var dcIndicator = CMenu.AddItem((new MenuItem("st_cheats_dci", "Disable Cast Indicator")).SetValue(false));
            dcIndicator.SetValue(Hacks.DisableCastIndicator);
            dcIndicator.ValueChanged += (sender, args) => Hacks.DisableCastIndicator = args.GetNewValue<bool>();

            CMenu.AddItem((new MenuItem("st_cheats_separator_4", "It disables the display of your skill ranges")));

            Utility.DelayAction.Add(100, () => Settings.LibraryMenu.AddSubMenu(CMenu));
        }
    }
}