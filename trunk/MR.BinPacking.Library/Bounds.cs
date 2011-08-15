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
        public static int L1(IEnumerable<int> elements, int c)
        {
            return (int)Math.Ceiling(elements.Sum(el => (decimal)el) / c);
        }

        //lower bound from 8.19 ("full" L2)
        public static int L2(IEnumerable<int> elements, int c)
        {
            var elems = from elem in elements
                        where 2 * elem <= c
                        orderby elem descending
                        select elem;

            if (elems.Count() == 0)
                return elements.Count();

            decimal jStarSum = elems.Sum(el => (decimal)el);
            int L1 = L1(elements, c);
            int maxL = L1;

            var distinctElems = elems.Distinct();
            foreach (var alpha in distinctElems)
            {
                IEnumerable<int> J1 = elements.Where(e => e > c - alpha);
                IEnumerable<int> J2 = elements.Where(e => (c - alpha >= e) && (2 * e > c));
                IEnumerable<int> J3 = elements.Where(e => (c >= 2 * e) && (e >= alpha));

                int tmp1 = J1.Count() + J2.Count();
                decimal tmp2 = J2.Count() * c - J2.Sum(el => (decimal)el);

                int L2 = tmp1 + Math.Max(0, (int)Math.Ceiling((double)(J3.Sum(el => (decimal)el) - tmp2) / c));
                maxL = Math.Max(maxL, L2);

                int test = tmp1 + (int)Math.Ceiling((double)(jStarSum - tmp2) / c);
                if (test <= maxL)
                    return maxL;
            }

            return maxL;
        }
    }
}
