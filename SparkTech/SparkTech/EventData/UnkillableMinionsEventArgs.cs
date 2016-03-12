namespace SparkTech.EventData
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;

    /// <summary>
    /// The UnkillableMinions event args
    /// </summary>
    public class UnkillableMinionsEventArgs : EventArgs
    {
        /// <summary>
        /// The minions which can't be killed using nothing but auto-attacks
        /// </summary>
        public readonly IList<Obj_AI_Minion> Minions;

        internal UnkillableMinionsEventArgs(IList<Obj_AI_Minion> minions)
        {
            Minions = minions;
        }
    }
}