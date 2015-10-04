namespace SparkTech.Resources
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using LeagueSharp;
    using LeagueSharp.Common;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static class MenuST
    {
        internal static Menu LibraryMenu = new Menu("[ST] Core", "SparkTech", true);

        internal static void Instance()
        {
            #region F5 Settings

            var F5Settings = new Menu("F5 Settings", "f5settings");
            {
                
            }
            LibraryMenu.AddSubMenu(F5Settings);

            #endregion

            #region
            
            #endregion

            LibraryMenu.AddToMainMenu();

            #region Variables

            Settings.SkipNoUpdate = false;
            Settings.UpdateCheck = true;
            
            #endregion

            Boot.FireOnInit();

            Updater.Check("SparkTech");
        }
    }
}
