namespace SparkTech.EventData
{
    using System;
    using System.Collections.Generic;

    using LeagueSharp;

    /// <summary>
    /// Contains informations about the suggested spell farm
    /// </summary>
    public class SpellFarmSuggestedEventArgs : EventArgs
    {
        /// <summary>
        /// The minions that the orbwalker needs help with
        /// </summary>
        public readonly IReadOnlyCollection<Obj_AI_Minion> Minions; 

        /// <summary>
        /// Initializes a new instance of the <see cref="SpellFarmSuggestedEventArgs"/> class
        /// </summary>
        /// <param name="minions">The list of the minions</param>
        internal SpellFarmSuggestedEventArgs(IReadOnlyCollection<Obj_AI_Minion> minions)
        {
            Minions = minions;
        }
    }
}