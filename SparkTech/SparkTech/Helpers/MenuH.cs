namespace SparkTech.Helpers
{
    using System.Linq;

    using LeagueSharp.SDK.Core.UI.IMenu;
    using LeagueSharp.SDK.Core.UI.IMenu.Abstracts;
    using LeagueSharp.SDK.Core.UI.IMenu.Values;

    public static class MenuH
    {
        /// <summary>
        /// Adds the separator to the specified <see cref="Menu"/> instance
        /// </summary>
        /// <param name="menu">The <see cref="Menu"/> instance</param>
        /// <param name="text">The custom text to be displayed</param>
        public static void AddSeparator(this Menu menu, string text = null)
        {
            menu.Add(new MenuSeparator(StringH.SeparatorText, text ?? string.Empty));
        }

        /// <summary>
        /// Gets the <see cref="TMenuComponent"/> globally
        /// </summary>
        /// <typeparam name="TMenuComponent">The desired type</typeparam>
        /// <param name="name">The <see cref="AMenuComponent"/> name</param>
        /// <returns></returns>
        public static TMenuComponent GetGlobally<TMenuComponent>(string name) where TMenuComponent : AMenuComponent
        {
            return
                MenuManager.Instance.Menus.SelectMany(rootmenu => rootmenu.Components.Values)
                    .OfType<TMenuComponent>()
                    .FirstOrDefault(component => component.Name == name);
        }

        /// <summary>
        /// Returns a root menu instance if found one with the specified name; null otherwise
        /// </summary>
        /// <param name="name">The menu name to be searched. (Not the display one)</param>
        /// <returns></returns>
        public static Menu GetRootMenu(string name) => MenuManager.Instance[name];
    }
}