namespace SparkTech.SparkWalker
{
    using System.Collections.Generic;

    using LeagueSharp;

    public class HitData
    {
        internal static readonly HitData Yes = new HitData(true), No = new HitData(false);

        public readonly List<GameObject> SoldiersInRange;

        public readonly bool PlayerInRange;

        internal HitData(bool byplayer, List<GameObject> soldiers = null)
        {
            PlayerInRange = byplayer;

            SoldiersInRange = soldiers ?? new List<GameObject>(0);
        }
    }
}