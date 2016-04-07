namespace SparkTech.Executors
{
    using System;

    using SparkTech.EventData;

    /// <summary>
    /// Derive from it in order to create a self-injecting miscallenous (utility) class
    /// </summary>
    public abstract class Miscallenous
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Miscallenous"/> class
        /// </summary>
        protected Miscallenous()
        {
            Core.GameUpdate += args =>
                {
                    try
                    {
                        GameUpdate(args);
                    }
                    catch (Exception ex)
                    {
                        ex.Catch();
                    }
                };
        }

        /// <summary>
        /// Executed when the game engine updates, contains useful event data
        /// </summary>
        /// <param name="args">The custom event data</param>
        protected virtual void GameUpdate(GameUpdateEventArgs args) { }
    }
}