namespace SparkTech
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("ReSharper", "UseNullPropagation")]
    public static class Events
    {
        public delegate void OnMenuCreated(EventArgs args);
        public static event OnMenuCreated OnInit;

        internal static void FireOnInit()
        {
            if (OnInit != null)
            {
                OnInit(new EventArgs());
            }
        }
    }
}
