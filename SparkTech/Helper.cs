namespace SparkTech
{
    using System;
    using System.Linq;
    using LeagueSharp;

    public class Helper
    {
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }

        public static bool IsPet(Obj_AI_Minion obj)
        {
            var pets = new[]
                           {
                               "annietibbers", "elisespiderling", "heimertyellow", "heimertblue", "leblanc",
                               "malzaharvoidling", "shacobox", "shaco", "yorickspectralghoul", "yorickdecayedghoul",
                               "yorickravenousghoul", "zyrathornplant", "zyragraspingplant"
                           };
            return pets.Contains(obj.CharData.BaseSkinName.ToLower());
        }
    }
}