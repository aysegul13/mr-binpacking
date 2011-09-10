﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using MR.BinPacking.Library.Experiment;
using System.ComponentModel;
using MR.BinPacking.Library.Utils;
using System.Diagnostics;
using MR.BinPacking.Library.Base;
using MR.BinPacking.Library;
using System.Threading;
using System.Reflection;

namespace MR.BinPacking.App
{
    /// <summary>
    /// Interaction logic for ExperimentProgressWindow.xaml
    /// </summary>
    public partial class ExperimentProgressWindow : Window
    {
        ExperimentParams experimentParams = null;
        ExperimentInstance experimentInstance = null;
        Thread workerThread = null;

        public ExperimentProgressWindow(ExperimentParams prms, ExperimentInstance instance)
        {
            InitializeComponent();

            experimentParams = prms;
            experimentInstance = instance;
        }

        void ReportProgress(ExperimentState state)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        laRepeat.Content = String.Format("Powtórzenie: {0}", state.Repeat + 1);
                        laStep.Content = String.Format("Liczba elementów: {0}", state.N);

                        laDistribution.Content = String.Format("Rozkład: {0}", ExperimentUtils.GetDistributionDisplayName(state.Distribution));
                        laSorting.Content = String.Format("Sortowanie: {0}", ExperimentUtils.GetSortingDisplayName(state.Sorting));

                        laAlgorithm.Content = String.Format("Algorytm: {0}", state.AlgorithmName);

                        laPart.Content = String.Format("Krok: {0} z {1}", state.ActualSample, state.Samples);
                        pbPart.Maximum = 1.0;
                        pbPart.Value = (double)(state.ActualSample) / state.Samples;
                    }));
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        void Complete(ExperimentResult result)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        guiChart.DataSource = result;

                        spExperimentProgress.Visibility = pbPart.Visibility = bRun.Visibility = bCancel.Visibility = Visibility.Collapsed;
                        guiChart.Visibility = Visibility.Visible;

                        this.Width = 600;
                        this.Height = 450;

                        guiChart.GetParamsAndRefresh();
                    }));
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        List<Sample> DoWorkInternal(ExperimentInstance I, ExperimentParams prms, int samplesCount, int sampleNumber, out int counter, int R)
        {
            counter = 0;

            List<Sample> result = new List<Sample>();
            int N = I.Elements.Count;

            foreach (var S in prms.Sortings)
            {
                List<int> elements = Experiment.GetElementsWithSorting(I.Elements, S);

                for (int i = 0; i < prms.Algorithms.Count; i++)
                {
                    BaseAlgorithm A = prms.Algs[i];
                    ExperimentState state = new ExperimentState()
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

        void DoWorkFile()
        {
            try
            {
                ExperimentParams prms = experimentParams;
                int samplesCount = prms.Sortings.Count * prms.Algorithms.Count;
                //int samplesCount = prms.Repeat * (((prms.MaxN - prms.MinN) / prms.Step) + 1) * prms.Distributions.Count * prms.Sortings.Count * prms.Algorithms.Count;
                int sampleNumber = 0;

                ExperimentResult result = new ExperimentResult()
                {
                    Params = prms,
                    Samples = new List<Sample>()
                };


                int counter = 0;
                List<Sample> samples = DoWorkInternal(experimentInstance, prms, samplesCount, sampleNumber, out counter, 0);
                if (samples == null)
                    return;

                result.Samples.AddRange(samples);

                Complete(result);
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        void DoWorkGenerator()
        {
            try
            {
                ExperimentParams prms = experimentParams;
                int samplesCount = prms.Repeat * (((prms.MaxN - prms.MinN) / prms.Step) + 1) * prms.Distributions.Count * prms.Sortings.Count * prms.Algorithms.Count;
                int sampleNumber = 0;

                ExperimentResult result = new ExperimentResult()
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

                            ExperimentInstance I = new ExperimentInstance()
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
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if ((workerThread != null) && workerThread.IsAlive)
                    workerThread.Abort();
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }

            bCancel.IsEnabled = false;
            bRun.IsEnabled = false;
        }

        private void bRun_Click(object sender, RoutedEventArgs e)
        {
            bCancel.IsEnabled = true;
            bRun.IsEnabled = false;

            try
            {
                if (experimentInstance == null)
                    workerThread = new Thread(this.DoWorkGenerator);
                else
                    workerThread = new Thread(this.DoWorkFile);

                workerThread.Start();

                //TODO: this may cause errors!!!
                while (!workerThread.IsAlive) ;
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                if ((workerThread != null) && workerThread.IsAlive)
                    workerThread.Abort();
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}