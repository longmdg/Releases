﻿namespace Testing
{
    using System;

    using SparkTech;
    using SparkTech.Resources;

    internal static class Program
    {
        private static void Main()
        {
            Console.WriteLine("Initing Testing...");

            Boot.Initialize();

            Boot.OnInit += eventArgs => Comms.Print("Testing - OnInit Fired!");

            Updater.Check("Testing");

            Console.WriteLine("Testing inited!");
        }
    }
}
