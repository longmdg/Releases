namespace Testing
{
    using System;
    using System.Reflection;

    using SparkTech;
    using SparkTech.Resources;

    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine("Initing Testing...");

            Boot.Initialize();

            var assemblyName = Assembly.GetExecutingAssembly().GetName();

            Boot.OnInit += eventArgs =>
                {
                    Comms.Print("Testing - OnInit Fired!");
                    Updater.Check("Testing", assemblyName);
                };

            Console.WriteLine("Testing inited!");
        }
    }
}
