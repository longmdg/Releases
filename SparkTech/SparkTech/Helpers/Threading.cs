namespace SparkTech.Helpers
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Threading;
    
    using SparkTech.Executors;

    /// <summary>
    /// Provides additional methods related to threading
    /// </summary>
    public class Threading : Miscallenous
    {
        public Threading()
        {
            CreateThreads(Settings.InitialThreadAmount);
        }

        /// <summary>
        /// Represents the currently executing process
        /// </summary>
        public static readonly Process Process = Process.GetCurrentProcess();

        /// <summary>
        /// Gets the current thread amount
        /// </summary>
        public static int ActiveThreadCount => Process.Threads.Count;

        /// <summary>
        /// The current <see cref="System.Windows.Threading.Dispatcher"/>
        /// </summary>
        private static readonly Dispatcher Dispatcher = Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Delays an action thread-safe
        /// </summary>
        /// <param name="minms">The expected delay in miliseconds</param>
        /// <param name="callback">The callback</param>
        /// <param name="mainThread">Determines whether the action should be performed on main thread</param>
        public static void ExecuteWithMinimumDelay(Action callback, int minms, bool mainThread = true)
        {
            var currentD = Dispatcher.CurrentDispatcher;

            Task.Run(delegate
                {
                    try
                    {
                        Thread.Sleep(minms);

                        (mainThread ? Dispatcher : currentD).BeginInvoke(callback);
                    }
                    catch (Exception ex)
                    {
                        ex.Catch();
                    }
                });
        }

        /// <summary>
        /// Creates a specified number of threads for the thread pool to be used
        /// </summary>
        /// <param name="amount">The amount of threads to be created</param>
        public static void CreateThreads(ushort amount)
        {
            try
            {
                ThreadStart func = delegate { };

                while (amount-- > 0)
                {
                    new Thread(func).Start();
                }
            }
            catch (Exception ex)
            {
                ex.Catch();
            }
        }
    }
}