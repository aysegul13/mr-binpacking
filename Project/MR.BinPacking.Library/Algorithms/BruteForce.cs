using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;

namespace MR.BinPacking.Library.Algorithms
{
    public class BruteForce : BaseAlgorithm
    {
        public BruteForce()
        {
            Name = "Brute Force";
        }

        public override Instance Execute(List<int> elements, int binSize)
        {
            Result = new Instance(binSize);
            Result.Elements = elements;
            
            best = null;

            L2 = Bounds.L2(elements, binSize);
            L3 = Bounds.L3(elements, binSize);
            Search(elements, 0, new List<List<int>>(), binSize);

            
            throw new NotImplementedException();
        }

        //Instance best = null;

        int L2;
        int L3;
        List<List<int>> best = null;
        int nodes = 0;

        void Search(List<int> elements, int elemIdx, List<List<int>> bins, int c)
        {
            nodes++;

            if (elemIdx < elements.Count)
            {
                for (int i = 0; i < bins.Count; i++)
                {
                    if (c - bins[i].Sum() >= elements[elemIdx])
                    {
                        bins[i].Add(elements[elemIdx]);
                        Search(elements, elemIdx + 1, bins, c);
                        bins[i].RemoveAt(bins[i].Count - 1);
                    }
                }

                if ((best == null) || (bins.Count < best.Count - 1))
                {
                    bins.Add(new List<int>());
                    bins.Last().Add(elements[elemIdx]);

                    List<int> tmpElems = elements.GetRange(elemIdx + 1, elements.Count - (elemIdx + 1));
                    foreach (var bin in bins)
                        tmpElems.Add(bin.Sum());

                    int currL2 = Bounds.L2(tmpElems, c);
                    if (currL2 <= L2)
                    {
                        L2 = currL2;

                        int currL3 = Bounds.L3(tmpElems, c);
                        if (currL3 <= L3)
                        {
                            L3 = currL3;
                            Search(elements, elemIdx + 1, bins, c);
                        }
                    }

                    bins.RemoveAt(bins.Count - 1);
                }
            }
            else
            {
                if ((best == null) || (bins.Count < best.Count))
                {
                    best = new List<List<int>>();
                    foreach (var bin in bins)
                        best.Add(new List<int>(bin));
                }
            }
        }
    }
}
