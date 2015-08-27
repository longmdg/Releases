using SparkTech;

namespace Testing
{
    internal class Program
    {
        internal const string AssemblyName = "Testing";

        private static void Main()
        {
            Updater.Init();
            Load.Library();
        }
    }
}
