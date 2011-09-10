using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using MR.BinPacking.Library.Algorithms;

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
            int L1 = Bounds.L1(elements, c);
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

        public static int L3(IEnumerable<int> elements, int c)
        {
            List<int> w = new List<int>(elements);
            w.Sort();
            w.Reverse();

            List<int> N = new List<int>();
            for (int i = 0; i < w.Count; i++)
                N.Add(i + 1);

            int zr = 0;
            int[] B = new int[w.Count];
            int L3 = 0;

            while (true)
            {
                if (N.Count <= 1)
                    break;

                B = Reduction.MTRP(w, N, c, B, ref zr);

                int L2;
                if (B.Where(b => b == 0).Count() == 0)
                    L2 = 0;
                else
                    L2 = Bounds.L2(N.Select(sn => w.ElementAt(sn - 1)), c);

                L3 = Math.Max(L3, zr + L2);

                if (N.Count > 0)
                    N.RemoveAt(N.Count - 1);
            }

            return L3;
        }
    }
}
