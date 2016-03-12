namespace SparkTech.Base
{
    using System;

    using LeagueSharp.SDK.Core.UI.IMenu;

    using SparkTech.Helpers;
    using SparkTech.SparkWalker;

    internal static class OrbwalkingManager
    {
        static OrbwalkingManager()
        {
            var managerMenu = new Menu("Orbwalking", "st_core_orbwalking");
            {
                Orbwalker = new Orbwalker(managerMenu);
                xSaliceWalker.AddToMenu(managerMenu);
                

                managerMenu.AddSeparator();

                var handle =
                    managerMenu.AddItem(
                        new MenuItem("st_core_orbwalking_handle", "Active").SetValue(
                            new StringList(Enum.GetNames(typeof(Orbwalker)))));

                var infoItem = managerMenu.AddItem(new MenuItem("st_core_orbwalking_info", "Used orbwalker: Unknown"));

                Helper.UsedOrbwalker = (Orbwalker)Enum.Parse(typeof(Orbwalker), handle.GetValue<StringList>().SelectedValue);

                handle.ValueChanged += (sender, args) =>
                {
                    var newOrb = (Orbwalker)Enum.Parse(typeof(Orbwalker), args.GetNewValue<StringList>().SelectedValue);

                    Helper.UsedOrbwalker = newOrb;

                    infoItem.DisplayName = $"Used orbwalker: {newOrb.ToString()}";
                };
            }

            Core.Menu.AddSubMenu(managerMenu);
        }
    }
}