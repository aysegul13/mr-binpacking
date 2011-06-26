using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;

namespace MR.BinPacking.Library
{
    public static class Bounds
    {
        //lower bound from 8.14
        public static int LowerBound(List<int> elements, int c)
        {
            double test = elements.Sum(el => (double)el);

            return (int)Math.Ceiling((double)elements.Sum() / c);
        }

        //lower bound from 8.19
        public static int StrongerLowerBound(List<int> elements, int c, int alpha)
        {
            if ((alpha < 0) || (2 * alpha > c))
                throw new ArgumentOutOfRangeException();

            IEnumerable<int> J1 = elements.Where(e => e > c - alpha);
            IEnumerable<int> J2 = elements.Where(e => (c - alpha >= e) && (2 * e > c));
            IEnumerable<int> J3 = elements.Where(e => (c >= 2 * e) && (e >= alpha));
            
            return J1.Count() + J2.Count() + Math.Max(0, (int)Math.Ceiling((double)(J3.Sum() - (J2.Count() * c - J2.Sum())) / c));
        }
    }
}
