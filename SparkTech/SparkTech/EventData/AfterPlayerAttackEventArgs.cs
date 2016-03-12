namespace SparkTech.EventData
{
    using System;

    using LeagueSharp;

    /// <summary>
    /// The instance containing the AfterPlayerAttack event args
    /// </summary>
    public class AfterPlayerAttackEventArgs : EventArgs
    {
        /// <summary>
        /// The target
        /// </summary>
        public readonly AttackableUnit Target;

        internal AfterPlayerAttackEventArgs(AttackableUnit target)
        {
            Target = target;
        }
    }
}