using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPackaging.Library
{
    public class Probability
    {
        public static double RandomUniform(Random random)
        {
            return random.NextDouble();
        }

        public static double RandomGauss(Random random)
        {
            double r1 = Math.Sqrt(-2.0 * Math.Log(1.0 - random.NextDouble()));
            double r2 = 2.0 * Math.PI * random.NextDouble();

            double x = r1 * Math.Cos(r2);
            //double y = r1 * Math.Sin(r2);

            return x;
        }

        public static double RandomExponential(Random random)
        {
            return -Math.Log(1.0 - random.NextDouble());
        }
    }
}
