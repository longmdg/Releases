namespace SparkTech.SparkWalker
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
        /// <param name="shouldWait">Determines whether this instance should wait</param>
        internal TargetData(bool shouldWait)
        {
            Target = null;

            ShouldWait = shouldWait;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TargetData"/> class
        /// </summary>
        /// <param name="target">The target</param>
        internal TargetData(AttackableUnit target)
        {
            Target = target;

            ShouldWait = false;
        }
    }
}