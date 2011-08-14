using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;

namespace MR.BinPacking.Library.Algorithms
{
    public class MTRP : BaseAlgorithm
    {
        public MTRP()
        {
            Name = "Algorytm Redukcji";
        }

        public override Instance Execute(List<int> elements, int binSize)
        {
            Result = new Instance(binSize);
            Result.Elements = elements;

            List<int> w = new List<int>(elements);
            w.Sort();
            w.Reverse();

            List<int> N = new List<int>();
            for (int i = 0; i < w.Count; i++)
                N.Add(i + 1);

            int zr = 0;
            int[] B = new int[w.Count];

            List<int> R = new List<int>();
            while (true)
            {
                B = MTRP.Execute(w, N, binSize, B, ref zr);
                if (N.Count == 0)
                    break;

                R.Add(w[N.Last() - 1]);
                N.RemoveAt(N.Count - 1);
            }

            for (int i = 0; i < zr; i++)
                Result.Bins.Add(new Bin(binSize));

            for (int i = 0; i < B.Length; i++)
            {
                if (B[i] > 0)
                    Result.Bins[B[i] - 1].Insert(w[i]);
            }



            for (int i = 0; i < Result.Bins.Count; i++)
            {
                while ((R.Count > 0) && (Result.Bins[i].FreeSpace() >= R[0]))
                {
                    Result.Bins[i].Insert(R[0]);
                    R.RemoveAt(0);
                }
            }

            //pack R using NextFit
            if (R.Count > 0)
            {
                NextFit nextFit = new NextFit() { IsPresentation = false };
                Instance tmpInst = nextFit.Execute(R, binSize);
                Result.Bins.AddRange(tmpInst.Bins);
            }

            return Result;
        }

        public static int[] Execute(List<int> w, List<int> N, int c, int[] B, ref int zr)
        {
            w.Sort();
            w.Reverse();

            List<int> Ndiff = new List<int>(N);
            List<int> Ndash = new List<int>();

            do
            {
                int j = N.Where(n => !Ndash.Contains(n)).Min();
                List<int> Nprime = new List<int>(N);
                Nprime.Remove(j);

                List<int> F = new List<int>();

                int k = 0;
                int l = Nprime.Count;
                int tmpSum = 0;
                while (tmpSum <= c - w[j - 1])
                {
                    k++;

                    int q = l - k + 1;

                    if ((Nprime.Count == 0) || (q == 0))
                        break;

                    tmpSum += w[Nprime[q - 1] - 1];
                }
                k--;

                if (k == 0)
                {
                    F.Clear();
                    F.Add(j);
                }
                else
                {
                    int jstar = Nprime.Where(h => w[j - 1] + w[h - 1] <= c).Min();

                    if ((k == 1) || (w[j - 1] + w[jstar - 1] == c))
                    {
                        F.Clear();
                        F.Add(j);
                        F.Add(jstar);
                    }
                    else if (k == 2)
                    {
                        int bestSum = 0;
                        int a = 0;
                        int b = 0;
                        for (int r = 1; r < Nprime.Count; r++)
                        {
                            for (int s = Nprime.Count; s > r; s--)
                            {
                                int sum = w[Nprime[r - 1] - 1] + w[Nprime[s - 1] - 1];
                                if ((sum <= c - w[j - 1]) && (sum > bestSum))
                                {
                                    bestSum = sum;
                                    a = r;
                                    b = s;
                                }
                            }
                        }

                        int ja = Nprime[a - 1];
                        int jb = Nprime[b - 1];


                        if (w[jstar - 1] >= w[ja - 1] + w[jb - 1])
                        {
                            F.Clear();
                            F.Add(j);
                            F.Add(jstar);
                        }
                        else if ((w[jstar - 1] == w[ja - 1]) && ((b - a <= 2) || (w[j - 1] + w[(jb - 1) - 1] + w[(jb - 2) - 1] > c)))
                        {
                            F.Clear();
                            F.Add(j);
                            F.Add(ja);
                            F.Add(jb);
                        }
                    }
                }

                if (F.Count == 0)
                {
                    if (!Ndash.Contains(j))
                        Ndash.Add(j);
                }
                else
                {
                    zr++;

                    foreach (var h in F)
                    {
                        B[h - 1] = zr;
                        N.Remove(h);
                    }
                }

            } while (N.Where(n => !Ndash.Contains(n)).Count() > 0);

            return B;
        }
    }
}
