namespace SparkTech.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using LeagueSharp;
    using LeagueSharp.Common;

   // [SuppressMessage("ReSharper", "ConvertPropertyToExpressionBody")]
    internal static class Helper
    {
        private static int temp;

        // ReSharper disable once InconsistentNaming
        public static bool IsFullHD = Drawing.Height == 1920 && Drawing.Width == 1080;
        
        internal static int NextNumber
        {
            get
            {
                return ++temp;
            }
        }
    }
}
