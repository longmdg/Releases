namespace SparkTech.Enumerations
{
    /// <summary>
    /// Represents the type of the missile check
    /// </summary>
    public enum MissileCheckType
    {
        /// <summary>
        /// Both missiles and math will be considered
        /// </summary>
        Strict,

        /// <summary>
        /// Both missiles and math will be considered
        /// </summary>
        Combined,

        /// <summary>
        /// Only math will be considered
        /// </summary>
        Math,

        /// <summary>
        /// Only missile will be considered
        /// </summary>
        Missile
    }
}