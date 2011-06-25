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
