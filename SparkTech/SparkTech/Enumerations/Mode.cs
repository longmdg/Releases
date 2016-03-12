namespace SparkTech.Enumerations
{
    /// <summary>
    /// Represents informations regarding the current orbwalking mode
    /// </summary>
    public enum Mode
    {
        /// <summary>
        /// The orbwalker is performing a combo
        /// </summary>
        Combo,

        /// <summary>
        /// The orbwalker is clearing the minions
        /// </summary>
        LaneClear,

        /// <summary>
        /// The orbwalker is harassing
        /// </summary>
        Harass,

        /// <summary>
        /// The orbwalker finishes minions
        /// </summary>
        LastHit,

        /// <summary>
        /// The orbwalker freezes the lane
        /// </summary>
        Freeze,

        /// <summary>
        /// The orbwalker flees
        /// </summary>
        Flee,

        /// <summary>
        /// The orbwalker is slacking
        /// </summary>
        None
    }
}