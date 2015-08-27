using System;
using System.Diagnostics.CodeAnalysis;
using LeagueSharp.Common;
// ReSharper disable ObjectCreationAsStatement

namespace SparkTech
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class Hacks
    {
        internal static Menu _hacksmenu;

        static Hacks()
        {
            _hacksmenu = new Menu("[ST] Hacks", "1", true);
            Utility.DelayAction.Add(1500, _hacksmenu.AddToMainMenu);
        }

        public class ZoomHack
        {
            static ZoomHack()
            {
                CustomEvents.Game.OnGameLoad += eventArgs =>
                {
                    new Hacks();
                    var zoomHack = _hacksmenu.AddItem((new MenuItem("2", "Enable ZoomHack")).SetValue(true));
                    _hacksmenu.AddItem((new MenuItem("3", "Warning: ZoomHack is extremely unsafe!")));
                    zoomHack.SetValue(LeagueSharp.Hacks.ZoomHack);
                    zoomHack.ValueChanged += delegate(object sender, OnValueChangeEventArgs args)
                    {
                        LeagueSharp.Hacks.ZoomHack = args.GetNewValue<bool>();
                    };
                    Comms.Print("<font color=\"#1eff00\">ZoomHack loaded!</font>");
                };
            }
        }

        public class DisableCastIndicator
        {
            static DisableCastIndicator()
            {
                var DCIndicator = _hacksmenu.AddItem((new MenuItem("dcindicator", "Disable Cast Indicator")).SetValue(false));
                DCIndicator.SetValue(LeagueSharp.Hacks.DisableCastIndicator);
                DCIndicator.ValueChanged += delegate(object sender, OnValueChangeEventArgs args)
                    {
                        LeagueSharp.Hacks.DisableCastIndicator = args.GetNewValue<bool>();
                    };
                Comms.Print("<font color=\"#1eff00\">Cast Indicator Disabler loaded!</font>");
            }
        }
    }
}
