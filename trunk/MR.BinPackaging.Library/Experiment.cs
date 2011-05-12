using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPackaging.Library.Base;
using System.Diagnostics;

namespace MR.BinPackaging.Library
{
    public class Statistics
    {
        public int N { get; set; }
        public int LowerBound { get; set; }
        public int StrongerLowerBound { get; set; }
        public int Result { get; set; }
        public long ExecutionTime { get; set; }
    }

    public enum StatField { LowerBound, StrongerLowerBound, Result, ExecutionTime };

    public class ExperimentParams
    {
        public int MinN { get; set; }
        public int MaxN { get; set; }
        public int Step { get; set; }
        public int Repeat { get; set; }
        public IListAlgorithm Algorithm { get; set; }
        public int BinSize { get; set; }
        public Distribution Dist { get; set; }
        public double MinVal { get; set; }
        public double MaxVal { get; set; }
    }

    public class Point2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point2D() : this(0.0, 0.0) { }
        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public static class Experiment
    {
        public static List<Point2D> GetCoordinates(List<List<Statistics>> stats, StatField field)
        {
            List<Point2D> result = new List<Point2D>();
            bool first = true;

            foreach (var roundStats in stats)
            {
                var roundVals = GetValues(roundStats, field).ToList();
                for (int i = 0; i < roundVals.Count; i++)
                {
                    if (first)
                        result.Add(new Point2D(roundStats[i].N, roundVals[i]));
                    else
                        result[i].Y += roundVals[i];
                }

                first = false;
            }

            if (stats.Count > 1)
            {
                for (int i = 0; i < result.Count; i++)
                    result[i].X /= stats.Count;
            }

            return result;
        }

        private static IEnumerable<long> GetValues(List<Statistics> stats, StatField field)
        {
            switch (field)
            {
                case StatField.LowerBound:
                    return stats.Select(s => (long)s.LowerBound);
                case StatField.StrongerLowerBound:
                    return stats.Select(s => (long)s.StrongerLowerBound);
                case StatField.ExecutionTime:
                    return stats.Select(s => s.ExecutionTime);
                default:
                    return stats.Select(s => (long)s.Result);
            }
        }

        public static List<List<Statistics>> ExecuteExperiment(ExperimentParams prms)
        {
            List<List<Statistics>> stats = new List<List<Statistics>>();
            for (int i = 0; i < prms.Repeat; i++)
            {
                List<Statistics> roundStats = new List<Statistics>();

                int currN = prms.MinN;
                while (currN <= prms.MaxN)
                {
                    int min = (int)Math.Ceiling(prms.MinVal * currN);
                    int max = (int)Math.Ceiling(prms.MaxVal * currN);

                    List<int> elements = Generator.GenerateData(currN, min, max, prms.Dist);
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    Instance result = prms.Algorithm.Execute(elements, prms.BinSize);
                    sw.Stop();

                    Statistics currStats = new Statistics()
                    {
                        N = currN,
                        LowerBound = Bounds.LowerBound(elements, prms.BinSize),
                        StrongerLowerBound = Bounds.StrongerLowerBound(elements, prms.BinSize, prms.BinSize / 2 - 1),
                        Result = result.Bins.Count(),
                        ExecutionTime = sw.ElapsedMilliseconds
                    };

                    roundStats.Add(currStats);

                    currN += prms.Step;
                }

                stats.Add(roundStats);
            }

            return stats;
        }
    }
}
