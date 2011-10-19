using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;
using MR.BinPacking.Library.Utils;
using MR.BinPacking.Library.Base;
using System.Diagnostics;

namespace MR.BinPacking.Library.Experiment
{
    public class Worker
    {
        ExpParams experimentParams = null;
        ExpInstance experimentInstance = null;

        public Action<ExpState> ReportProgress { get; set; }
        public Action<ExpResult> Complete { get; set; }
        public Action<Exception, string> ShowError { get; set; }

        public Worker(ExpParams prms, ExpInstance instance)
        {
            experimentParams = prms;
            experimentInstance = instance;
        }


        List<Sample> DoWorkInternal(ExpInstance I, ExpParams prms, int samplesCount, int sampleNumber, out int counter, int R)
        {
            counter = 0;

            List<Sample> result = new List<Sample>();
            int N = I.Elements.Count;

            foreach (var S in prms.Sortings)
            {
                List<int> elements = Generator.GetElementsWithSorting(I.Elements, S);

                for (int i = 0; i < prms.Algorithms.Count; i++)
                {
                    BaseAlgorithm A = prms.Algs[i];
                    ExpState state = new ExpState()
                    {
                        Repeat = R,
                        N = N,
                        Distribution = I.Dist,
                        Sorting = S,
                        AlgorithmName = A.Name,
                        Samples = samplesCount,
                        ActualSample = sampleNumber + counter + 1
                    };
                    ReportProgress(state);

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    Instance instanceResult = A.Execute(elements, prms.BinSize);
                    sw.Stop();

                    //int L1 = Bounds.LowerBound(elements, prms.BinSize);
                    int L2 = Bounds.L2(elements, prms.BinSize);
                    int res = instanceResult.Bins.Count();

                    double quality = (double)res / L2;
                    double error = 100.0 * (res - L2) / L2;

                    Sample stats = new Sample()
                    {
                        ID = sampleNumber + counter,
                        N = N,
                        Distribution = I.Dist,
                        Sorting = S,
                        Algorithm = prms.Algorithms[i],
                        Alg = prms.Algs[i],
                        QualityEstimation = quality,
                        ErrorEstimation = error,
                        Result = res,
                        ExecutionTime = sw.ElapsedMilliseconds
                    };

                    result.Add(stats);
                    counter++;
                }
            }

            return result;
        }

        public void DoWorkFile()
        {
            try
            {
                ExpParams prms = experimentParams;
                prms.BinSize = experimentInstance.BinSize;
                //int samplesCount = prms.Sortings.Count * prms.Algorithms.Count;
                int samplesCount = prms.Repeat * prms.Sortings.Count * prms.Algorithms.Count;
                //int samplesCount = prms.Repeat * (((prms.MaxN - prms.MinN) / prms.Step) + 1) * prms.Distributions.Count * prms.Sortings.Count * prms.Algorithms.Count;
                int sampleNumber = 0;

                ExpResult result = new ExpResult()
                {
                    Params = prms,
                    Samples = new List<Sample>()
                };


                for (int R = 0; R < prms.Repeat; R++)
                {
                    int counter = 0;
                    List<Sample> samples = DoWorkInternal(experimentInstance, prms, samplesCount, sampleNumber, out counter, R);
                    if (samples == null)
                        return;

                    result.Samples.AddRange(samples);
                    sampleNumber += counter;    //
                }

                Complete(result);
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                //MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        public void DoWorkGenerator()
        {
            try
            {
                ExpParams prms = experimentParams;
                int samplesCount = prms.Repeat * (((prms.MaxN - prms.MinN) / prms.Step) + 1) * prms.Distributions.Count * prms.Sortings.Count * prms.Algorithms.Count;
                int sampleNumber = 0;

                ExpResult result = new ExpResult()
                {
                    Params = prms,
                    Samples = new List<Sample>()
                };

                for (int R = 0; R < prms.Repeat; R++)
                {
                    int N = prms.MinN;
                    while (N <= prms.MaxN)
                    {
                        int min = (int)Math.Ceiling(prms.MinVal * prms.BinSize);
                        int max = (int)Math.Ceiling(prms.MaxVal * prms.BinSize);

                        foreach (var D in prms.Distributions)
                        {
                            List<int> elements = Generator.GenerateData(N, min, max, D);

                            ExpInstance I = new ExpInstance()
                            {
                                BinSize = prms.BinSize,
                                Dist = D,
                                Elements = elements
                            };

                            int counter = 0;
                            List<Sample> samples = DoWorkInternal(I, prms, samplesCount, sampleNumber, out counter, R);
                            if (samples == null)
                                return;

                            result.Samples.AddRange(samples);
                            sampleNumber += counter;
                        }

                        N += prms.Step;
                    }
                }

                Complete(result);
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                //MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
