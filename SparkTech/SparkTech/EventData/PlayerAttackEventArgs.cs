namespace SparkTech.EventData
{
    using System;

    using LeagueSharp;

    /// <summary>
    /// Contains the attack event args of any attack
    /// </summary>
    public class PlayerAttackEventArgs : EventArgs
    {
        /// <summary>
        /// The target
        /// </summary>
        public readonly AttackableUnit Target;

        internal PlayerAttackEventArgs(AttackableUnit target)
        {
            Target = target;
        }
    }
}