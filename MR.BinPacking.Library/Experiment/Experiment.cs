using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using System.Diagnostics;
using MR.BinPacking.Library.Utils;

namespace MR.BinPacking.Library.Experiment
{
    public static class Experiment
    {
        public static List<int> GetElementsWithSorting(List<int> elements, Sorting sorting)
        {
            switch (sorting)
            {
                case Sorting.Ascending:
                    elements.Sort();
                    return elements;
                case Sorting.Descending:
                    List<int> elementsSorted = new List<int>(elements);
                    elementsSorted.Sort((x, y) => y.CompareTo(x));
                    return elementsSorted;
                default:
                    return elements;
            }
        }

        public static List<Sample> Execute(ExperimentParams prms)
        {
            List<Sample> result = new List<Sample>();

            for (int R = 0; R < prms.Repeat; R++)
            {
                int N = prms.MinN;
                while (N <= prms.MaxN)
                {
                    int min = (int)Math.Ceiling(prms.MinVal * N);
                    int max = (int)Math.Ceiling(prms.MaxVal * N);

                    foreach (var D in prms.Distributions)
                    {
                        List<int> elements = Generator.GenerateData(N, min, max, D);
                        foreach (var S in prms.Sortings)
                        {
                            elements = GetElementsWithSorting(elements, S);

                            foreach (var A in prms.Algs)
                            {
                                Stopwatch sw = new Stopwatch();
                                sw.Start();
                                Instance instanceResult = A.Execute(elements, prms.BinSize);
                                sw.Stop();

                                Sample stats = new Sample()
                                {
                                    N = N,
                                    LowerBound = Bounds.LowerBound(elements, prms.BinSize),
                                    StrongerLowerBound = Bounds.StrongerLowerBound(elements, prms.BinSize, prms.BinSize / 2 - 1),
                                    Result = instanceResult.Bins.Count(),
                                    ExecutionTime = sw.ElapsedMilliseconds
                                };

                                result.Add(stats);
                            }
                        }
                    }

                    N += prms.Step;
                }
            }

            return result;
        }
    }
}
