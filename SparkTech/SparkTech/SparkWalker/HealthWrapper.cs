namespace SparkTech.SparkWalker
{
    using LeagueSharp;
    using LeagueSharp.SDK;

    public static class HealthWrapper
    {
        public static float GetPrediction(Obj_AI_Base unit, int time, int delay = 0, bool simulated = false, bool @default = false)
        {
            if (@default)
            {
                return Health.GetPrediction(unit, time, delay, simulated ? HealthPredictionType.Simulated : HealthPredictionType.Default);
            }

            /*
            
            
            */
        }

        public static int GetAggroCount(Obj_AI_Base unit, bool includeTurret = true)
        {
            
        }

        public static bool HasTurretAggro(Obj_AI_Base unit)
        {
            
        }

    }
}
