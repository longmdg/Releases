namespace SparkTech.EventData
{
    using System;

    using LeagueSharp;

    /// <summary>
    /// Contains the pre-attack event data
    /// </summary>
    public class BeforePlayerUnhumanAttackEventArgs : EventArgs
    {
        /// <summary>
        /// The target
        /// </summary>
        public readonly AttackableUnit Target;

        /// <summary>
        /// Gets or sets whether the attack should be cancelled
        /// </summary>
        public bool CancelAttack;

        /// <summary>
        /// Initializes a new instance of the <see cref="BeforePlayerUnhumanAttackEventArgs"/> class
        /// </summary>
        /// <param name="target">The target</param>
        internal BeforePlayerUnhumanAttackEventArgs(AttackableUnit target)
        {
            Target = target;
        }
    }
}