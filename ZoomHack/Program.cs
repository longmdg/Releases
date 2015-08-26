namespace ZoomHack
{
    static class Program
    {
        static LeagueSharp.Common.Menu _menu;
        static void Main()
        {
            LeagueSharp.Common.CustomEvents.Game.OnGameLoad += eventArgs =>
            {
                _menu = new LeagueSharp.Common.Menu("[ST] ZoomHack", "SparkTech", true);
                var zoomHack = _menu.AddItem((new LeagueSharp.Common.MenuItem("zoomhack", "Active!")).SetValue(true));
                zoomHack.SetValue(LeagueSharp.Hacks.ZoomHack);
                zoomHack.ValueChanged += delegate (object sender, LeagueSharp.Common.OnValueChangeEventArgs args)
                    {
                        LeagueSharp.Hacks.ZoomHack = args.GetNewValue<bool>();
                    };
                _menu.AddToMainMenu();
            };
        }
    }
}
