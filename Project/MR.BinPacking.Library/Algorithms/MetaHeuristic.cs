using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;

namespace MR.BinPacking.Library.Algorithms
{
    public class MetaHeuristic : BaseAlgorithm
    {
        public MetaHeuristic()
        {
            Name = "Metaheurystyka";
        }

        public override Instance Execute(List<int> elements, int binSize)
        {
            NextFit NF = new NextFit() { IsPresentation = false };
            Instance instance = NF.Execute(elements, binSize);
            int L2 = Bounds.L2(elements, binSize);

            int prev = instance.Bins.Count;
            int allAttempts = instance.Bins.Count;
            int attempts = 3;
            while ((attempts > 0) && (allAttempts > 0))
            {
                attempts--;
                allAttempts--;
                instance = Improve(instance, L2);

                if (instance.Bins.Count < prev)
                    attempts = 3;

                prev = instance.Bins.Count;
            }

            Result = instance;
            Result.Elements = elements;

            return Result;
        }

        Instance Improve(Instance instance, int L2)
        {
            int C1 = 3;

            if ((instance.Bins.Count == L2) || (instance.Bins.Count < C1))
                return instance;

            var binsSorted = (from bin in instance.Bins
                              orderby bin.FreeSpace() descending
                              select bin).ToList();

            List<int> tmpElems = new List<int>();
            for (int i = 0; i < C1; i++)
                tmpElems.AddRange(binsSorted[i].Elements);
            binsSorted = binsSorted.Skip(C1).ToList();

            int C2 = C1;
            if (binsSorted.Count >= C2)
            {
                binsSorted = (from bin in binsSorted
                              orderby bin.Elements.Count descending
                              select bin).ToList();
                for (int i = 0; i < C2; i++)
                    tmpElems.AddRange(binsSorted[i].Elements);
                binsSorted = binsSorted.Skip(C2).ToList();
            }

            Reduction exact = new Reduction();
            binsSorted.AddRange(exact.Execute(tmpElems, instance.BinSize).Bins);

            instance.Bins = binsSorted;
            return instance;
        }
    }
}
