using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace SparkTech
{
   public class Helper
    {
        public static int RandomNumber(int min, int max)
        {
            var random = new Random();
            return random.Next(min, max);
        }
    }
}
