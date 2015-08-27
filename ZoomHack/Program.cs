using LeagueSharp;
using LeagueSharp.Common;

namespace ZoomHack
{
    static class Program
    {
        public const string Version = "1.3.3.*";
        public const string DodgerBlue = "#1E90FF";
        private static Menu _menu;
        private static void Main()
        {
            CustomEvents.Game.OnGameLoad += eventArgs =>
            {
                _menu = new Menu("[ST] ZoomHack", "SparkTech", true);
                var zoomHack = _menu.AddItem((new MenuItem("zoomhack", "Use ZoomHack (Read Disclaimer)")).SetValue(true));
                _menu.AddItem((new MenuItem("1", "")));
                _menu.AddItem((new MenuItem("2", "DISCLAIMER:")));
                _menu.AddItem((new MenuItem("3", "This assembly will get you banned")));
                zoomHack.SetValue(Hacks.ZoomHack);
                zoomHack.ValueChanged += delegate (object sender, OnValueChangeEventArgs args)
                    {
                        Hacks.ZoomHack = args.GetNewValue<bool>();
                    };
                _menu.AddToMainMenu();
                Game.PrintChat("<font color=\"#1eff00\">ZoomHack loaded!</font> - care, it can get you banned.");
            };

        }
    }
}
