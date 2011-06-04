using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using System.Diagnostics;

namespace MR.BinPacking.Library
{
    public enum StatField { LowerBound, StrongerLowerBound, Result, ExecutionTime };

    public class Statistics
    {
        public int N { get; set; }
        public int LowerBound { get; set; }
        public int StrongerLowerBound { get; set; }
        public int Result { get; set; }
        public long ExecutionTime { get; set; }

        public long this[StatField index]
        {
            get
            {
                switch (index)
                {
                    case StatField.LowerBound:
                        return LowerBound;
                    case StatField.StrongerLowerBound:
                        return StrongerLowerBound;
                    case StatField.Result:
                        return Result;
                    case StatField.ExecutionTime:
                        return ExecutionTime;
                    default:
                        return N;
                }
            }
            private set {}
        }

        public void Add(Statistics stats)
        {
            LowerBound += stats.LowerBound;
            StrongerLowerBound += stats.StrongerLowerBound;
            Result += stats.Result;
            ExecutionTime += stats.ExecutionTime;
        }
    }

    public class ExperimentParams
    {
        public int MinN { get; set; }
        public int MaxN { get; set; }
        public int Step { get; set; }
        public int Repeat { get; set; }
        public int BinSize { get; set; }
        public Distribution Dist { get; set; }
        public double MinVal { get; set; }
        public double MaxVal { get; set; }

        public List<ListAlgorithm> Algorithms { get; set; }
    }

    public class AlgorithmResult
    {
        public List<Statistics> Result { get; set; }

        public AlgorithmResult()
        {
            Result = new List<Statistics>();
        }

        public Statistics this[int index]
        {
            get { return Result[index]; }
            set { Result[index] = value; }
        }
    }

    public class ExperimentResult
    {
        public ExperimentParams Params { get; set; }
        public List<AlgorithmResult> DataSeries { get; set; }

        public ExperimentResult()
        {
            DataSeries = new List<AlgorithmResult>();
        }

        public AlgorithmResult this[int index]
        {
            get { return DataSeries[index]; }
            set { DataSeries[index] = value; }
        }
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
        public static List<Point2D> GetCoordinates(AlgorithmResult stats, StatField field)
        {
            return stats.Result.Select(r => new Point2D(r.N, r[field])).ToList();
        }

        public static ExperimentResult ExecuteExperiment(ExperimentParams prms)
        {
            ExperimentResult result = new ExperimentResult();
            result.Params = prms;
            for (int k = 0; k < prms.Algorithms.Count; k++)
                result.DataSeries.Add(new AlgorithmResult());

            for (int i = 0; i < prms.Repeat; i++)
            {
                int j = 0;
                int currN = prms.MinN;
                while (currN <= prms.MaxN)
                {
                    int min = (int)Math.Ceiling(prms.MinVal * currN);
                    int max = (int)Math.Ceiling(prms.MaxVal * currN);
                    List<int> elements = Generator.GenerateData(currN, min, max, prms.Dist);

                    Stopwatch sw = new Stopwatch();

                    for (int k = 0; k < prms.Algorithms.Count; k++)
                    {
                        sw.Start();
                        Instance instanceResult = prms.Algorithms[k].Execute(elements, prms.BinSize);
                        sw.Stop();

                        Statistics currStats = new Statistics()
                        {
                            N = currN,
                            LowerBound = Bounds.LowerBound(elements, prms.BinSize),
                            StrongerLowerBound = Bounds.StrongerLowerBound(elements, prms.BinSize, prms.BinSize / 2 - 1),
                            Result = instanceResult.Bins.Count(),
                            ExecutionTime = sw.ElapsedMilliseconds
                        };

                        if (result[k].Result.Count <= j)
                            result[k].Result.Add(currStats);
                        else
                            result[k][j].Add(currStats);

                        sw.Reset();
                    }

                    currN += prms.Step;
                    j++;
                }
            }

            for (int k = 0; k < result.DataSeries.Count; k++)
            {
                for (int j = 0; j < result[k].Result.Count; j++)
                {
                    Statistics stats = result[k][j];
                    stats.LowerBound = (int)Math.Ceiling((double)stats.LowerBound / prms.Repeat);
                    stats.StrongerLowerBound = (int)Math.Ceiling((double)stats.StrongerLowerBound / prms.Repeat);
                    stats.Result = (int)Math.Ceiling((double)stats.Result / prms.Repeat);
                    stats.ExecutionTime = stats.ExecutionTime / prms.Repeat;
                }
            }

            return result;
        }
    }
}
