using System;
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

namespace MR.BinPacking.App
{
    /// <summary>
    /// Interaction logic for ExperimentProgressWindow.xaml
    /// </summary>
    public partial class ExperimentProgressWindow : Window
    {
        ExperimentParams experimentParams = null;
        ExperimentInstance experimentInstance = null;
        BackgroundWorker bw = null;

        public ExperimentProgressWindow(ExperimentParams prms, ExperimentInstance instance)
        {
            InitializeComponent();

            experimentParams = prms;
            experimentInstance = instance;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bw = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);

            if (experimentInstance == null)
                bw.DoWork += new DoWorkEventHandler(bw_DoWorkGenerator);
            else
                bw.DoWork += new DoWorkEventHandler(bw_DoWorkFile);
        }

        List<Sample> DoWorkInternal(BackgroundWorker worker, DoWorkEventArgs e, ExperimentInstance I, ExperimentParams prms, int samplesCount, int sampleNumber, out int counter, int R)
        {
            counter = 0;

            List<Sample> result = new List<Sample>();
            int N = I.Elements.Count;

            foreach (var S in prms.Sortings)
            {
                List<int> elements = Experiment.GetElementsWithSorting(I.Elements, S);

                //TODO: algorytm z innego źródła
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
                    worker.ReportProgress(0, state);

                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return null;
                    }

                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    Instance instanceResult = A.Execute(elements, prms.BinSize);
                    sw.Stop();

                    //int L1 = Bounds.LowerBound(elements, prms.BinSize);
                    int L2 = Bounds.L2(elements, prms.BinSize);
                    int res = instanceResult.Bins.Count();

                    double quality = (double)res / L2;
                    double error = 100.0 * (res - L2) / L2;

                    //TODO: dopisać info o algorytmie
                    Sample stats = new Sample()
                    {
                        ID = sampleNumber + counter,
                        N = N,
                        Distribution = I.Dist,
                        Sorting = S,
                        Algorithm = prms.Algorithms[i],
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

        void bw_DoWorkFile(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            //ExperimentParams prms = e.Argument as ExperimentParams;
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
            List<Sample> samples = DoWorkInternal(worker, e, experimentInstance, prms, samplesCount, sampleNumber, out counter, 0);
            if (samples == null)
                return;

            result.Samples.AddRange(samples);

            e.Result = result;
        }

        void bw_DoWorkGenerator(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            //ExperimentParams prms = e.Argument as ExperimentParams;
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
                        List<Sample> samples = DoWorkInternal(worker, e, I, prms, samplesCount, sampleNumber, out counter, 0);
                        if (samples == null)
                            return;

                        result.Samples.AddRange(samples);
                        sampleNumber += counter;
                    }

                    N += prms.Step;
                }
            }

            e.Result = result;
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ExperimentState state = e.UserState as ExperimentState;

            //laRepeat.Content = String.Format("Powtórzenie: {0}", state.Repeat + 1);
            laStep.Content = String.Format("Liczba elementów: {0}", state.N);

            //TODO: poprawić wyświetlane nazwy dla rozkładu i sortowania
            laDistribution.Content = String.Format("Rozkład: {0}", state.Distribution.ToString());
            laSorting.Content = String.Format("Sortowanie: {0}", state.Sorting.ToString());

            laAlgorithm.Content = String.Format("Algorytm: {0}", state.AlgorithmName);

            laPart.Content = String.Format("Krok: {0} z {1}", state.ActualSample, state.Samples);
            pbPart.Maximum = 1.0;
            pbPart.Value = (double)(state.ActualSample) / state.Samples;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && (e.Error == null))
            {
                ExperimentResult result = e.Result as ExperimentResult;
                guiChart.DataSource = result;

                spExperimentProgress.Visibility = Visibility.Collapsed;
                guiChart.Visibility = Visibility.Visible;

                this.Width = 600;
                this.Height = 450;

                guiChart.GetParamsAndRefresh();
            }
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
        {
            if (bw.WorkerSupportsCancellation)
                bw.CancelAsync();
        }

        private void bRun_Click(object sender, RoutedEventArgs e)
        {
            if (!bw.IsBusy)
                bw.RunWorkerAsync(experimentParams);
        }
    }
}
