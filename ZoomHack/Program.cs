using SparkTech;
// ReSharper disable ObjectCreationAsStatement

namespace ZoomHack
{
    internal static class Program
    {
        internal const string AssemblyName = "ZoomHack";

        private static void Main()
        {
            Updater.Init();
            new Hacks.DisableCastIndicator();
            new Hacks.ZoomHack();
        }
    }
}
