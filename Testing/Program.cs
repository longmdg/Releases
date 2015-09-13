namespace Testing
{
    using SparkTech;

    internal class Program
    {
        internal const string AssemblyName = "Testing";

        private static void Main()
        {
            Updater.Init();
            object SparkTech = new Load();
        }
    }
}