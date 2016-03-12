namespace STAutoPlay
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// The main mode enumeration
    /// </summary>
    [Flags]
    [DefaultValue(Slacking)]
    public enum Mode
    {
        /// <summary>
        /// Player is charging at enemies or executing a combo
        /// </summary>
        Attacking = 0,

        /// <summary>
        /// Player is safely attacking other champions
        /// </summary>
        Defending = 1,

        /// <summary>
        /// Player is clearing the lane
        /// </summary>
        FarmingLane = 2,

        /// <summary>
        /// Player is clearing the jungle
        /// </summary>
        FarmingJungle = 3,

        /// <summary>
        /// Player is in minion - attacking mode
        /// </summary>
        Farming = FarmingLane | FarmingJungle,

        /// <summary>
        /// Player is looking forward returning to base
        /// </summary>
        Returning = 4,

        /// <summary>
        /// Player is waiting in the fountain - purchasing items or regenerating health
        /// </summary>
        Waiting = 5,

        /// <summary>
        /// Player is writing messages
        /// </summary>
        Slacking = 6
    }

    [DefaultValue(Spamming)]
    internal enum MessageType
    {
        Motivating,

        Random,

        Flaming,

        Spamming,

        EscapingEnemy,

        Joke,

        Responsive,

        EnemyLucker,

        MeLucker
    }
}