namespace SparkTech.EventData
{
    using System;

    using LeagueSharp;

    /// <summary>
    /// Contains the event data about target switching
    /// </summary>
    public class PlayerTargetSwitchEventArgs : EventArgs
    {
        /// <summary>
        /// The old target
        /// </summary>
        public readonly AttackableUnit OldTarget;

        /// <summary>
        /// The new target
        /// </summary>
        public readonly AttackableUnit NewTarget;

        internal PlayerTargetSwitchEventArgs(AttackableUnit oldTarget, AttackableUnit newTarget)
        {
            OldTarget = oldTarget;
            NewTarget = newTarget;
        }
    }
}