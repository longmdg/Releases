namespace Spaghetti_Katarina
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Security.Permissions;

    using SparkTech.Base;

    using LeagueSharp.Common;

    using SparkTech.Web;

    internal static class Program
    {
    //    private static Menu menu;

        public static SparkWalker Orbwalker;
        
        private static void Main(string[] a)
        {
            if (a == null || a.Any())
            {
                return;
            }

      //      new SecurityPermission(PermissionState.Unrestricted).Assert();

            CustomEvents.Game.OnGameLoad += delegate
            {
                new ActionUpdater(
                    "https://raw.githubusercontent.com/Wiciaki/Releases/master/SparkTech/Spaghetti_Katarina/Properties/AssemblyInfo.cs",
                    Assembly.GetAssembly(typeof(Program)))
                    .OnCheckPerformed += args => args.Notify();

                //  (menu ?? (menu = new Menu("Asd", "asd123", true))).AddToMainMenu();

                //  orb = new SparkTech.Base.SparkWalker(menu);
            };
        }
    }
}
