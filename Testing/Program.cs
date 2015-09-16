namespace Testing
{
    using SparkTech;

    internal class Program
    {
        private const string AssemblyName = "Testing";

        private static void Main()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Load();
            Updater.Check(AssemblyName);
        }
    }
}