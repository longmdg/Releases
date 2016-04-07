namespace SparkTech.SparkWalker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LeagueSharp;
    using LeagueSharp.SDK;

    using SharpDX;

    using SparkTech.Cache;

    /// <summary>
    ///     Gets informations determining how the <see cref="E:Player" /> can currently strike the target using attacks
    /// </summary>
    public class HitData
    {
        #region Fields

        /// <summary>
        ///     Determines whether the <see cref="E:Player" /> is in range
        /// </summary>
        public readonly bool PlayerInRange;

        /// <summary>
        ///     Gets the list of soldiers that have the target in their range
        /// </summary>
        public readonly List<Obj_AI_Minion> SoldiersInRange;

        /// <summary>
        ///     The unit the check was requested for
        /// </summary>
        public readonly Obj_AI_Base Unit;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="HitData" /> class
        /// </summary>
        /// <param name="unit"></param>
        public HitData(Obj_AI_Base unit)
        {
            Unit = unit;

            if (unit == null || !unit.IsValid)
            {
                SoldiersInRange = new List<Obj_AI_Minion>(0);
                return;
            }

            var enemyPos = unit.ServerPosition.ToVector2();
            var range = ObjectCache.Player.AttackRange + ObjectCache.Player.BoundingRadius + unit.BoundingRadius;

            if (Core.ChampionName == "Caitlyn"
                && unit.Buffs.Any(
                    buff =>
                    buff.Name.Equals("caitlynyordletrapinternal", StringComparison.InvariantCultureIgnoreCase)
                    && buff.Caster.IsMe))
            {
                range += 650f;
            }

            PlayerInRange = Vector2.DistanceSquared(enemyPos, ObjectCache.Player.ServerPosition.ToVector2()) <= range * range;

            SoldiersInRange = Soldiers.Ready.FindAll(
                soldier =>
                {
                    var stab = soldier.BoundingRadius + Soldiers.BaseWarriorRange + unit.BoundingRadius;

                    return Vector2.DistanceSquared(soldier.ServerPosition.ToVector2(), enemyPos) <= stab * stab;
                });
        }

        #endregion
    }
}