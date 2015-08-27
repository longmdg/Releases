using LeagueSharp;
using LeagueSharp.Common;

namespace ZoomHack
{
    static class Program
    {
        public const string Version = "1.3.3.*";
        private static Menu _menu;
        private static void Main()
        {
            CustomEvents.Game.OnGameLoad += eventArgs =>
            {
                _menu = new Menu("[ST] ZoomHack", "SparkTech", true);
                var zoomHack = _menu.AddItem((new MenuItem("zoomhack", "Active!")).SetValue(true));
                // blank space
                // DISCLAIMER
                // THis will probably get you banned.
                _menu.AddItem((new MenuItem("", "")));
                _menu.AddItem((new MenuItem("", "DISCLAIMER:")));
                _menu.AddItem((new MenuItem("", "This assembly will get you banned")));
                zoomHack.SetValue(Hacks.ZoomHack);
                zoomHack.ValueChanged += delegate (object sender, OnValueChangeEventArgs args)
                    {
                        Hacks.ZoomHack = args.GetNewValue<bool>();
                    };
                _menu.AddToMainMenu();

                Game.PrintChat("Unsafe ZoomHack loaded! Turn on in menu to activate!");
            };
        }
    }
}
