using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;

namespace MR.BinPacking.Library.Algorithms
{
    public class Exact : BaseAlgorithm
    {
        public Exact()
        {
            Name = "Algorytm Dokładny";
        }

        public override Instance Execute(List<int> elements, int binSize)
        {
            Result = new Instance(binSize);
            Result.Elements = elements;
            
            best = null;
            L2 = Bounds.L2(elements, binSize);
            L3 = Bounds.L3(elements, binSize);

            var elems = (from elem in elements
                         orderby elem descending
                         select elem).ToList();

            Search(elems, 0, new List<List<int>>(), binSize);

            foreach (var bin in best)
            {
                Bin newBin = new Bin(binSize);
                newBin.Elements.AddRange(bin);
                Result.Bins.Add(newBin);
            }

            return Result;
        }


        int L2;
        int L3;
        List<List<int>> best = null;

        bool Search(List<int> elements, int elemIdx, List<List<int>> bins, int c)
        {
            if (elemIdx < elements.Count)
            {
                for (int i = 0; i < bins.Count; i++)
                {
                    if (c - bins[i].Sum() >= elements[elemIdx])
                    {
                        bins[i].Add(elements[elemIdx]);
                        if (Search(elements, elemIdx + 1, bins, c))
                            return true;
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
                            if (Search(elements, elemIdx + 1, bins, c))
                                return true;
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

                    if ((best.Count == L2) && (best.Count == L3))
                        return true;
                }
            }

            return false;
        }
    }
}
