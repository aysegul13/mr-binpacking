using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPackaging.Library.Base;

namespace MR.BinPackaging.Library
{
    public static class Bounds
    {
        //lower bound from 8.14
        public static int LowerBound(List<int> elements, int c)
        {
            return (int)Math.Ceiling((double)elements.Sum() / c);
        }

        //lower bound from 8.19
        public static int StrongerLowerBound(List<int> elements, int c, int alpha)
        {
            if ((alpha < 0) || (2 * alpha > c))
                throw new ArgumentOutOfRangeException();

            List<int> J1 = elements.Where(e => e > c - alpha).ToList();
            List<int> J2 = elements.Where(e => (c - alpha >= e) && (2 * e > c)).ToList();
            List<int> J3 = elements.Where(e => (c >= 2 * e) && (e >= alpha)).ToList();

            return J1.Count + J2.Count + Math.Max(0, (int)Math.Ceiling((double)(J3.Sum() - (J2.Count * c - J2.Sum())) / c));
        }
    }
}
