using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using MR.BinPacking.Library;
using MR.BinPacking.Library.Algorithms;

namespace MR.BinPacking.Test
{
    class Program
    {
        //static int L3(IEnumerable<int> elements, int c)
        //{
        //    int L3 = 0;
        //    int z = 0;

        //    List<int> Wdash = new List<int>(elements);
        //    List<int> Ndash = new List<int>();
        //    for (int j = 0; j < elements.Count(); j++)
        //        Ndash.Add(j + 1);

        //    int[] B = new int[Wdash.Count];


        //    while (Ndash.Count >= 1)
        //    {
        //        B = Reduction.MTRP(Wdash, Ndash, c, B, ref z);
        //        int k = 0;

        //        for (int j = 1; j <= Ndash.Count; j++)
        //        {
        //            if (B[j - 1] == 0)
        //            {
        //                k++;
        //                Wdash[k - 1] = Wdash[j - 1];
        //            }
        //        }

        //        Ndash = Ndash.Take(k).ToList();

        //        int L2 = 0;
        //        if (Ndash.Count == 0)
        //            L2 = 0;
        //        else
        //            L2 = Bounds.L2(Wdash, c);

        //        L3 = Math.Max(L3, z + L2);

        //        if (Ndash.Count > 0)
        //            Ndash.RemoveAt(Ndash.Count - 1);
        //    }

        //    return L3;
        //}

        static int L3(IEnumerable<int> elements, int c)
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
                {
                    L2 = 0;
                }
                else
                {
                    L2 = Bounds.L2(N.Select(sn => w.ElementAt(sn - 1)), c);
                }

                L3 = Math.Max(L3, zr + L2);

                if (N.Count > 0)
                    N.RemoveAt(N.Count - 1);
            }

            return L3;
        }

        static void Main(string[] args)
        {
            int c = 100;
            Instance I = new Instance(c);
            I.Elements = new List<int>() { 99, 94, 79, 64, 50, 46, 43, 37, 32, 19, 18, 7, 6, 3 };

            //int L1 = Bounds.L1(I.Elements, c);
            //int L2 = Bounds.L2(I.Elements, c);

            //List<int> N = new List<int>();
            //for (int i = 0; i < I.Elements.Count; i++)
            //    N.Add(i + 1);

            //int zr = 0;
            //int[] B = new int[I.Elements.Count];
            //B = Reduction.MTRP(I.Elements, N, c, B, ref zr);

            //Reduction red = new Reduction();
            //red.Execute(I.Elements, c);

            int L3 = Program.L3(I.Elements, c);
        }
    }
}
