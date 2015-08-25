﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace SparkTech
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class Settings
    {
        internal static bool Debug = true;

        public static Menu LibraryMenu;

        internal static void LoadStuff()
        {
            LibraryMenu = new Menu("[ST] Core", "SparkTech", true);

            /*
            F5Settings -> Default Settings?
            F5Settings -> Subsciption Manager
            F5Settings -> Target Selector
            F5Settings -> Orbwalker
            F5Settings -> Prediction
            
            Messaging -> Message output (PrintChat, Notifs, Console)
            Messaging -> Delay / Time

            more 2 come

            */

            LibraryMenu.AddToMainMenu();
        }
    }
}
