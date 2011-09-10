﻿using System;
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

            //int L3 = Program.L3(I.Elements, c);

            

            //Random rand = new Random();
            //List<int> elemsRaw = new List<int>();
            //for (int i = 0; i < 300; i++)
            //    elemsRaw.Add(rand.Next(1, 30));

            //var elems = (from elem in elemsRaw
            //             orderby elem descending
            //             select elem).ToList();


            //List<int> elemsRelaxed = new List<int>() { 29, 26, 26, 22, 20, 19, 90, 34, 33 };
            //int L2 = Bounds.L2(elems, c);
            //int L2r = Bounds.L2(elemsRelaxed, c);

            c = 10;

            List<int> elems = GetElements();
            MetaHeuristic mh = new MetaHeuristic();
            mh.Execute(elems, c);

            //Exact bf = new Exact();
            //bf.Execute(elems, c);
        }

        static List<int> GetElements()
        {
            //List<int> elems = new List<int>() { 50, 60, 40, 20, 30 };
            //List<int> elems = new List<int>() { 50, 60, 40, 20, 30, 25 };
            //List<int> elems = new List<int>() { 99, 94, 79, 64, 50, 46, 43, 37, 32, 19, 18, 7, 6, 3 };
            //List<int> elems = new List<int>() { 60, 50, 40, 40, 30, 30, 30, 25, 25, 20, 20 };
            //List<int> elems = new List<int>() { 49, 41, 34, 33, 29, 26, 26, 22, 20, 19 };
            //List<int> elems = new List<int>() { 60, 55, 55, 51, 45, 43, 34, 25, 13, 12 };
            //List<int> elems = new List<int>() { 35, 46, 40, 35, 46, 28, 20, 46, 23, 25, 33, 41, 32, 36 };
            //List<int> elems = new List<int>() { 5, 4, 2, 9, 4, 2, 3, 2, 6, 10, 8, 1, 10, 1, 5, 9, 10, 9, 6, 3 };
            int[] elems = { 6, 2, 7, 7, 3, 4, 1, 1, 7, 8, 8, 6, 1, 10, 9, 5, 2, 2, 6, 10, 5, 5, 2, 1, 5, 6, 6, 3, 8, 3 };

            return elems.ToList();
        }
    }
}