namespace SparkTech.EventData
{
    using System;

    /// <summary>
    /// The main event delegate used for handling most of the event data instances
    /// </summary>
    /// <typeparam name="TEventArgs">The destination event args</typeparam>
    /// <param name="args">The event data</param>
    public delegate void EventDataHandler<in TEventArgs>(TEventArgs args) where TEventArgs : EventArgs;
}