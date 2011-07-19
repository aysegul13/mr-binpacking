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
        //public static List<Point2D> GetCoordinates(AlgorithmResult stats, StatField field)
        //{
        //    return stats.Result.Select(r => new Point2D(r.N, r[field])).ToList();
        //}

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

        //public static ExperimentResult ExecuteExperiment(ExperimentParams prms)
        //{
        //    ExperimentResult result = new ExperimentResult();
        //    result.Params = prms;
        //    for (int k = 0; k < prms.Algs.Count; k++)
        //        result.DataSeries.Add(new AlgorithmResult());

        //    for (int i = 0; i < prms.Repeat; i++)
        //    {
        //        int j = 0;
        //        int currN = prms.MinN;
        //        while (currN <= prms.MaxN)
        //        {
        //            int min = (int)Math.Ceiling(prms.MinVal * currN);
        //            int max = (int)Math.Ceiling(prms.MaxVal * currN);
        //            //TODO: wybieranie wszystkich rozkładów, a nie tylko pierwszego
        //            //List<int> elements = Generator.GenerateData(currN, min, max, prms.Dist);
        //            List<int> elements = Generator.GenerateData(currN, min, max, prms.Distributions.First());
        //            if (prms.Sortings.First() == Sorting.Ascending)
        //            {
        //                elements.Sort();
        //            }
        //            else if (prms.Sortings.First() == Sorting.Descending)
        //            {
        //                List<int> elementsSorted = new List<int>(elements);
        //                elementsSorted.Sort((x, y) => y.CompareTo(x));
        //                elements = elementsSorted;
        //            }

        //            Stopwatch sw = new Stopwatch();

        //            for (int k = 0; k < prms.Algs.Count; k++)
        //            {
        //                sw.Start();
        //                Instance instanceResult = prms.Algs[k].Execute(elements, prms.BinSize);
        //                sw.Stop();

        //                Statistics currStats = new Statistics()
        //                {
        //                    N = currN,
        //                    LowerBound = Bounds.LowerBound(elements, prms.BinSize),
        //                    StrongerLowerBound = Bounds.StrongerLowerBound(elements, prms.BinSize, prms.BinSize / 2 - 1),
        //                    Result = instanceResult.Bins.Count(),
        //                    ExecutionTime = sw.ElapsedMilliseconds
        //                };

        //                if (result[k].Result.Count <= j)
        //                    result[k].Result.Add(currStats);
        //                else
        //                    result[k][j].Add(currStats);

        //                sw.Reset();
        //            }

        //            currN += prms.Step;
        //            j++;
        //        }
        //    }

        //    for (int k = 0; k < result.DataSeries.Count; k++)
        //    {
        //        for (int j = 0; j < result[k].Result.Count; j++)
        //        {
        //            Statistics stats = result[k][j];
        //            stats.LowerBound = (int)Math.Ceiling((double)stats.LowerBound / prms.Repeat);
        //            stats.StrongerLowerBound = (int)Math.Ceiling((double)stats.StrongerLowerBound / prms.Repeat);
        //            stats.Result = (int)Math.Ceiling((double)stats.Result / prms.Repeat);
        //            stats.ExecutionTime = stats.ExecutionTime / prms.Repeat;
        //        }
        //    }

        //    return result;
        //}
    }
}
