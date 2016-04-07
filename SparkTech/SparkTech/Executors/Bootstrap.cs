namespace SparkTech.Executors
{
    using LeagueSharp.SDK;
    
    using SparkTech.Web;

    public abstract class Bootstrap<T> where T : class
    {
        /// <summary>
        ///     Initializes this instance
        /// </summary>
        /// <param name="updatePath">
        ///     The assembly will check for the updates of the assembly.
        ///     <para>This works with raw GitHub links only</para>
        /// </param>
        protected static void Initialize(string updatePath = null)
        {
            var assembly = typeof(T).Assembly;

            Handler.Register(assembly);

            if (updatePath == null)
            {
                return;
            }

            new ActionUpdater(updatePath, assembly).CheckPerformed += args =>
            {
                Events.OnLoad += delegate
                {
                    args.Notify();
                };
            };
        }
    }
}