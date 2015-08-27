﻿using System;
using System.Diagnostics.CodeAnalysis;
using LeagueSharp;
using LeagueSharp.Common;

namespace SparkTech
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    class Hack
    {
        private static Menu _menu;
        static Hack()
        {
            if (!Settings.MenuLoaded)
            {
                return;
            }
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

                BanMenu.AddItem((new MenuItem("2", "Prevents you from seeing the range of your skills")));
                BanMenu.AddItem((new MenuItem("3", "")));

                var ZoomHack =
                    BanMenu.AddItem((new MenuItem("zoomhack", "Use ZoomHack (bannable!)")).SetValue(false));
                ZoomHack.SetValue(Hacks.ZoomHack);
                ZoomHack.ValueChanged +=
                    delegate(object sender, OnValueChangeEventArgs args)
                    {
                        Hacks.ZoomHack = args.GetNewValue<bool>();
                    };

                BanMenu.AddItem((new MenuItem("confirm", "Care: ZoomHack can get you banned!")));
            }
            Settings.LibraryMenu.AddSubMenu(BanMenu);
        }

        public static void LoadZoomHackStandalone()
        {
            CustomEvents.Game.OnGameLoad += eventArgs =>
            {
                _menu = new Menu("[ST] ZoomHack", "SparkTech", true);
                var zoomHack = _menu.AddItem((new MenuItem("zoomhack", "Enabled (Check Disclaimer)")).SetValue(true));
                _menu.AddItem((new MenuItem("1", "")));
                _menu.AddItem((new MenuItem("2", "DISCLAIMER:")));
                _menu.AddItem((new MenuItem("3", "This assembly will get you banned!")));
                _menu.AddItem((new MenuItem("4", "You accept that fact by using it.")));
                zoomHack.SetValue(Hacks.ZoomHack);
                zoomHack.ValueChanged += delegate (object sender, OnValueChangeEventArgs args)
                {
                    Hacks.ZoomHack = args.GetNewValue<bool>();
                };
                _menu.AddToMainMenu();
                Game.PrintChat("<font color=\"#1eff00\">ZoomHack loaded!</font>");
            };
        }
    }
}