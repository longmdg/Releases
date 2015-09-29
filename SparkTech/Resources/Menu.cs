namespace SparkTech.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using LeagueSharp;

    public class Menu
    {
        static Menu()
        {
            
        }

        internal static void Create()
        {
            SparkTech.OnFinalize += OnFinalize;
        }

        private static void OnFinalize(EventArgs args)
        {
            Game.PrintChat("Finalized!");
            Updater.Check("SparkTech");
        }
    }
}
