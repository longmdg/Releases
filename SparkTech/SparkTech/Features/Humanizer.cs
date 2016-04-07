/*
namespace SparkTech.Features
{
    using System.Collections.Generic;

    using LeagueSharp;
    using LeagueSharp.SDK.Core.UI.IMenu;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    using SharpDX;

    using SparkTech.Executors;

    using static System.String;

    public class Humanizer : IMenuPiece
    {
        private static readonly List<Zone> Zones = new List<Zone>();

        public static readonly Zone Zone;

        private static bool enabled;

        public static bool Enabled
        {
            get
            {
                return enabled;
            }
            set
            {
                enabled = value;

                if (menu != null)
                {
                    menu[""].GetValue<MenuBool>().Value = value;
                }
            }
        }

        public class Zone
        {
            public Zone()
            {
                Zones.Add(this);
            }

            public bool IsAllowed(Vector3 position)
            {
                
            }

            public bool CanBeAttacked(AttackableUnit unit)
            {

            }
        }

        private static Menu menu;

        /// <summary>
        /// The submenu to be added to the root
        /// </summary>
        public Menu Piece
        {
            get
            {
                menu = new Menu("st_core_humanizer", Empty);
                {
                    var disabledItem = menu.Add(new MenuBool("st_core_humanizer_disable", Empty));

                }
                return menu;
            }
        }
    }
}*/