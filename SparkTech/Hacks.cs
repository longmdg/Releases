using LeagueSharp.Common;

// ReSharper disable ObjectCreationAsStatement

namespace SparkTech
{
    public class Hacks
    {
        public static bool HackInited;

        internal static Menu HacksMenu;

        internal static void AddMenu()
        {
            if (HackInited)
            {
                return;
            }
            HackInited = true;
            HacksMenu = new Menu("Hacks", "hacks1");
        }

        public class ZoomHack
        {
            static ZoomHack()
            {
                CustomEvents.Game.OnGameLoad += eventArgs =>
                {
                    AddMenu();
                    MenuItem zoomHack = HacksMenu.AddItem((new MenuItem("hacks2", "Enable ZoomHack")).SetValue(false));
                    HacksMenu.AddItem((new MenuItem("hacks3", "Warning: ZoomHack is extremely unsafe!")));
                    zoomHack.SetValue(LeagueSharp.Hacks.ZoomHack);
                    zoomHack.ValueChanged += delegate (object sender, OnValueChangeEventArgs args)
                    {
                        LeagueSharp.Hacks.ZoomHack = args.GetNewValue<bool>();
                    };
                };
            }
        }

        public class DisableCastIndicator
        {
            static DisableCastIndicator()
            {
                AddMenu();
                MenuItem dcIndicator = HacksMenu.AddItem((new MenuItem("dcindicator", "Disable Cast Indicator")).SetValue(false));
                dcIndicator.SetValue(LeagueSharp.Hacks.DisableCastIndicator);
                dcIndicator.ValueChanged += delegate (object sender, OnValueChangeEventArgs args)
                    {
                        LeagueSharp.Hacks.DisableCastIndicator = args.GetNewValue<bool>();
                    };
            }
        }
    }
}