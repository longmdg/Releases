namespace SparkTech.SparkWalker
{
    using System;

    using SparkTech.Cache;
    using SparkTech.Utils;

    /// <summary>
    /// The windup assistant
    /// </summary>
    internal static class Windup
    {
        /// <summary>
        /// Gets the current ping
        /// </summary>
        /// <param name="ping">The ping value</param>
        /// <param name="stillStutters">Determines whether to increase the value</param>
        /// <returns></returns>
        internal static int Get(int ping, bool stillStutters)
        {
            var result = Math.Max(ping - 20 + ping < 20 ? 20 : ping / ping >= 40 ? 10 : 20, Offset);

            if (stillStutters)
            {
                result *= 3;
            }

            return result;
        }

        /// <summary>
        /// The minimal windup value
        /// </summary>
        private static readonly int Offset = SetOffset();

        /// <summary>
        /// Gets the champion offset
        /// </summary>
        /// <returns></returns>
        private static int SetOffset()
        {
            switch (ObjectCache.Player.ChampionName())
            {
                case "Alistar":
                case "Amumu":
                case "Anivia":
                case "Annie":
                case "Bard":
                case "Blitzcrank":
                case "Cassiopeia":
                case "Cho'Gath":
                case "Darius":
                case "Dr. Mundo":
                case "Fiddlesticks":
                case "Galio":
                case "Garen":
                case "Gragas":
                case "Heimerdinger":
                case "Janna":
                case "Jarvan IV":
                case "Karthus":
                case "Kassadin":
                case "Kha'Zix":
                case "LeBlanc":
                case "Lee Sin":
                case "Leona":
                case "Lucian":
                case "Lulu":
                case "Lux":
                case "Malphite":
                case "Malzahar":
                case "Maokai":
                case "Morgana":
                case "Nami":
                case "Nasus":
                case "Nautilus":
                case "Nunu":
                case "Orianna":
                case "Pantheon":
                case "Rammus":
                case "Rek'Sai":
                case "Rengar":
                case "Rumble":
                case "Sejuani":
                case "Singed":
                case "Sion":
                case "Sona":
                case "Soraka":
                case "Swain":
                case "Syndra":
                case "Tahm Kench":
                case "Taric":
                case "Thresh":
                case "Tryndamere":
                case "Twisted Fate":
                case "Udyr":
                case "Urgot":
                case "Veigar":
                case "Vel'Koz":
                case "Vladimir":
                case "Volibear":
                case "Wukong":
                case "Xerath":
                case "Xin Zhao":
                case "Yorick":
                case "Zac":
                case "Ziggs":
                case "Zilean":
                    return 50;
                default:
                    return 20;
            }
        }
    }
}
