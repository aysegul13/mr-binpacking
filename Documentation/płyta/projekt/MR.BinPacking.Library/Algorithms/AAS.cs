using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using MR.BinPacking.Library.Utils;
using System.Diagnostics;
using System.Reflection;
using System.Collections;

namespace MR.BinPacking.Library.Algorithms
{
    public class AAS : BaseAlgorithm
    {
        public AAS()
        {
            Name = "Asymptotyczny Schemat Aproksymacyjny";
        }


        List<int[]> T = new List<int[]>();

        void PreparePackings(int[] weights, int[] counts, int c)
        {
            T = new List<int[]>();
            int[] packing = new int[weights.Length];
            Work(weights, counts, packing, 0, c);
        }

        int GetSum(int[] weights, int[] packing)
        {
            int sum = 0;
            for (int i = 0; i < packing.Length; i++)
                sum += weights[i] * packing[i];

            return sum;
        }

        void Work(int[] weights, int[] counts, int[] packing, int idx, int c)
        {
            for (int i = idx; i < weights.Length; i++)
            {
                packing[i]++;

                if ((packing[i] <= counts[i]) && (GetSum(weights, packing) <= c))
                {
                    //Print(packing);
                    T.Add((int[])packing.Clone());
                    Work(weights, counts, packing, i, c);
                }

                packing[i]--;
            }
        }

        public double Epsilon { get; set; }
        public override Instance Execute(List<int> elements, int binSize)
        {
            return Execute(elements.ToArray(), Epsilon, binSize);
        }

        int GetIndex(int i, int[] I, int m, int h)
        {
            return I.Length + 1 - (m - i + 1) * h;
        }

        Instance Execute(int[] I, double epsilon, int c)
        {
            Result = new Instance(c);
            Result.Elements = I.ToList();

            Array.Sort(I);

            #region STEP 1

            double gamma = c * epsilon / (epsilon + 1);
            int h = (int)Math.Ceiling((decimal)epsilon * I.Sum(el => (decimal)el) / c);

            #endregion

            //get L
            int[] L = I.Where(e => e < gamma).ToArray();
            int m = (int)Math.Floor((double)(I.Length - L.Length) / h);

            //get R
            int[] R;

            if (m > 0)
            {
                int ym = GetIndex(m, I, m, h);
                R = I.Skip(ym).ToArray();
            }
            else
            {
                R = I.Skip(L.Length).ToArray();
            }

            #region pack R

            foreach (var item in R)
            {
                Bin bin = new Bin(c);
                bin.Insert(item);
                Result.Bins.Add(bin);
            }

            #endregion


            #region pack M (K's)

            if (m > 0)
            {
                //get M
                int[] M = new int[m];
                for (int i = 1; i <= m; i++)
                    M[i - 1] = I[GetIndex(i, I, m, h) - 1];

                //prepare packings
                int[] Kcounts = new int[m];
                int skip = L.Length;
                List<List<int>> K = new List<List<int>>();
                Kcounts[0] = GetIndex(1, I, m, h) - L.Length;    //K0 can have less elements
                List<int> K0 = I.Skip(skip).Take(Kcounts[0]).ToList();
                K.Add(K0);
                skip += Kcounts[0];
                for (int i = 1; i < m; i++)
                {
                    Kcounts[i] = h;
                    List<int> Ki = I.Skip(skip).Take(h).ToList();
                    K.Add(Ki);
                    skip += h;
                }

                PreparePackings(M, Kcounts, c);

                int packings = T.Count;

                try
                {
                    //LP
                    int lp = lpsolve.make_lp(0, packings);

                    //add constraints
                    for (int i = 0; i < m; i++)
                    {
                        //try
                        //{
                            double[] constraintRaw = T.Select(t => (double)t[i]).ToArray();
                            double[] constraint = new double[packings + 1];
                            constraintRaw.CopyTo(constraint, 1);
                            lpsolve.add_constraint(lp, constraint, lpsolve.lpsolve_constr_types.GE, Kcounts[i]);
                        //}
                        //catch (Exception exc)
                        //{

                        //}
                    }

                    //set objective function
                    double[] obj = new double[packings + 1];
                    for (int i = 1; i < packings + 1; i++)
                        obj[i] = 1;
                    lpsolve.set_obj_fn(lp, obj);

                    //solve
                    lpsolve.solve(lp);

                    int N = lpsolve.get_Ncolumns(lp);
                    double[] resultLP = new double[N];
                    if (!lpsolve.get_variables(lp, resultLP))
                        throw new Exception("Wystąpił błąd!!!");

                    for (int i = 0; i < N; i++)
                    {
                        int x = (int)Math.Floor((decimal)resultLP[i]);
                        if (x == 0)
                            continue;

                        int[] packing = T[i];
                        for (int p = 0; p < x; p++)
                        {
                            bool addBin = true;
                            for (int elemNumber = 0; elemNumber < packing.Length; elemNumber++)
                            {
                                if (K[elemNumber].Count < packing[elemNumber])
                                {
                                    addBin = false;
                                    break;
                                }
                            }

                            if (!addBin)
                                continue;

                            Bin bin = new Bin(c);
                            for (int elemNumber = 0; elemNumber < packing.Length; elemNumber++)
                            {
                                //try
                                //{
                                    bin.Elements.AddRange(K[elemNumber].Take(packing[elemNumber]));
                                    K[elemNumber].RemoveRange(0, packing[elemNumber]);
                                //}
                                //catch (Exception exc)
                                //{

                                //}
                            }
                            Result.Bins.Add(bin);
                        }
                    }
                }
                catch (Exception exc)
                {
                    throw exc;
                    //MessageBox.Show(exc.Message + exc.StackTrace, MethodBase.GetCurrentMethod().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                }

                List<int> leftK = new List<int>();
                foreach (var actualK in K)
                    leftK.AddRange(actualK);

                //pack leftK using NextFit
                if (leftK.Count > 0)
                {
                    NextFit nextFit = new NextFit() { IsPresentation = false };
                    Instance tmpInst = nextFit.Execute(leftK, c);
                    Result.Bins.AddRange(tmpInst.Bins);
                }
            }

            #endregion

            #region pack L

            List<int> leftL = new List<int>(L);
            for (int i = 0; i < Result.Bins.Count; i++)
            {
                while ((leftL.Count > 0) && (Result.Bins[i].FreeSpace() >= leftL[0]))
                {
                    Result.Bins[i].Insert(leftL[0]);
                    leftL.RemoveAt(0);
                }
            }

            //pack leftL using NextFit
            if (leftL.Count > 0)
            {
                NextFit nextFit = new NextFit() { IsPresentation = false };
                Instance tmpInst = nextFit.Execute(leftL, c);
                Result.Bins.AddRange(tmpInst.Bins);
            }

            #endregion

            return Result;
        }
    }
}
