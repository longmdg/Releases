namespace SparkTech.SparkWalker
{
    using LeagueSharp;
    using LeagueSharp.SDK;

    public static class HealthWrapper
    {
        public static float GetPrediction(Obj_AI_Base unit, int time, int delay = 0, HealthPredictionType type = HealthPredictionType.Default)
        {
            return Health.GetPrediction(unit, time, delay, type);
        }

        public static bool HasTurretAggro(Obj_AI_Minion unit)
        {
            return Health.HasTurretAggro(unit);
        }
    }
}