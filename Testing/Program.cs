namespace Testing
{
    using System;

    using LeagueSharp;

    using SparkTech.Resources;

    using SparkTech = SparkTech.SparkTech;

    class Program
    {
        static void Main()
        {
            if (!SparkTech.Present)
            {
                return;
            }
            SparkTech.OnInit += OnInit;
            SparkTech.OnFinalize += OnFinalize;
        }

        private static void OnInit(EventArgs args)
        {
            Game.PrintChat("Testing OnInit!");
            SparkTech.FireOnFinalized();
        }
        private static void OnFinalize(EventArgs args)
        {
            Game.PrintChat("Testing OnFinalize!");
            Updater.Check("Testing");
        }
    }
}
