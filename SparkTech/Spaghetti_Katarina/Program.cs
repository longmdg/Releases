namespace Spaghetti_Katarina
{
    using System;

    using SparkTech.Executors;

    internal class Program : Bootstrap<Program>
    {
        private static void Main(string[] args)
        {
            if (args == null)
            {
                return;
            }

            foreach (var arg in args)
            {
                Console.WriteLine(arg);
            }

            Initialize();
        }
    }
}