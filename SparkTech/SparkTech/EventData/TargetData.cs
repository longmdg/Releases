namespace SparkTech.EventData
{
    using LeagueSharp;

    /// <summary>
    /// Provides informations about the target
    /// </summary>
    public class TargetData
    {
        /// <summary>
        /// The <see cref="AttackableUnit"/> instance
        /// </summary>
        public readonly AttackableUnit Target;

        /// <summary>
        /// Indicates whether the player should wait for a better target to be found
        /// </summary>
        public readonly bool ShouldWait;

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetData"/> class
        /// </summary>
        /// <param name="target">The target</param>
        /// <param name="shouldWait">The should wait value</param>
        internal TargetData(AttackableUnit target, bool shouldWait = false)
        {
            Target = target;

            ShouldWait = target == null && shouldWait;
        }
    }
}