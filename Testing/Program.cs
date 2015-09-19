namespace Testing
{
    using SparkTech;

    internal class Program
    {
        private const string Assembly = "Testing";

        private static void Main()
        {
            // ReSharper disable once ObjectCreationAsStatement
            new Load();
            Updater.Check(Assembly);
        }
    }
}