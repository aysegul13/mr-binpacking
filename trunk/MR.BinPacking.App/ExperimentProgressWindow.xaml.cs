using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MR.BinPacking.Library.Experiment;
using System.ComponentModel;
using MR.BinPacking.Library.Utils;
using System.Diagnostics;
using MR.BinPacking.Library.Base;
using MR.BinPacking.Library;
using MR.BinPacking.Library.Algorithms;

namespace MR.BinPacking.App
{
    /// <summary>
    /// Interaction logic for ExperimentProgressWindow.xaml
    /// </summary>
    public partial class ExperimentProgressWindow : Window
    {
        ExperimentParams experimentParams = null;
        BackgroundWorker bw = null;

        public ExperimentProgressWindow(ExperimentParams prms)
        {
            InitializeComponent();

            experimentParams = prms;
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
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);

            //ExperimentParams prms = new ExperimentParams()
            //{
            //    Algorithms = new List<ListAlgorithm>(),
            //    BinSize = 100,
            //    Dist = Distribution.Uniform,
            //    MinN = 100,
            //    MaxN = 1000,
            //    Step = 100,
            //    MinVal = 0.0,
            //    MaxVal = 1.0,
            //    Repeat = 2
            //};

            //prms.Algorithms.Add(new BestFitDecreasing() { IsPresentation = false });
            //prms.Algorithms.Add(new BestFit() { IsPresentation = false });


            //DataSeries = new List<List<Point2D>>();

            //ExperimentResult stats = Experiment.ExecuteExperiment(prms);
        }

        void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            ExperimentParams prms = e.Argument as ExperimentParams;

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
                    int min = Math.Max((int)Math.Ceiling(prms.MinVal * prms.BinSize), 1);
                    int max = (int)Math.Ceiling(prms.MaxVal * prms.BinSize);
                    //TODO: wybieranie wszystkich rozkładów, a nie tylko pierwszego
                    //List<int> elements = Generator.GenerateData(currN, min, max, prms.Dist);
                    List<int> elements = Generator.GenerateData(currN, min, max, prms.Distributions.First());

                    Stopwatch sw = new Stopwatch();

                    for (int k = 0; k < prms.Algorithms.Count; k++)
                    {
                        ExperimentState state = new ExperimentState()
                        {
                            Algorithm = k,
                            AlgorithmCount = prms.Algorithms.Count,
                            Step = (currN - prms.MinN) / prms.Step,
                            StepCount = ((prms.MaxN - prms.MinN) / prms.Step) + 1,
                            Repeat = i,
                            RepeatCount = prms.Repeat,
                            N = currN,
                            AlgorithmName = prms.Algorithms[k].Name
                        };
                        worker.ReportProgress(0, state);

                        if (worker.CancellationPending)
                        {
                            e.Cancel = true;
                            return;
                        }

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

            e.Result = result;
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ExperimentState state = e.UserState as ExperimentState;
            laRepeat.Content = String.Format("Powtórzenie: {0} z {1}", state.Repeat + 1, state.RepeatCount);
            laStep.Content = String.Format("Liczba elementów: {0}", state.N);
            laAlgorithm.Content = String.Format("Algorytm: {0} z {1} ({2})", state.Algorithm + 1, state.AlgorithmCount, state.AlgorithmName);

            pbRepeat.Maximum = 1.0;
            pbRepeat.Value = (double)(state.Repeat + 1) / state.RepeatCount;

            pbStep.Maximum = 1.0;
            pbStep.Value = (double)(state.Step + 1) / state.StepCount;

            pbAlgorithm.Maximum = 1.0;
            pbAlgorithm.Value = (double)(state.Algorithm + 1) / state.AlgorithmCount;
        }

        void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && (e.Error == null))
            {
                ExperimentResult stats = e.Result as ExperimentResult;

                guiChart.DataSeries = new List<List<Point2D>>();

                for (int i = 0; i < stats.DataSeries.Count; i++)
                    guiChart.DataSeries.Add(Experiment.GetCoordinates(stats[i], StatField.ExecutionTime));

                guiChart.AxisXIntervalWidth = stats.Params.Step;
                guiChart.AxisXMin = stats.Params.MinN;
                guiChart.Refresh();
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
            {
                ExperimentParams prms = new ExperimentParams()
                {
                    Algorithms = new List<ListAlgorithm>(),
                    BinSize = 100,
                    Distributions = new List<Distribution>(),
                    MinN = 100,
                    MaxN = 1000,
                    Step = 100,
                    MinVal = 0.0,
                    MaxVal = 1.0,
                    Repeat = 20
                };

                //prms.Algorithms.Add(new NextFit() { IsPresentation = false });
                prms.Algorithms.Add(new FirstFit() { IsPresentation = false });
                prms.Algorithms.Add(new BestFit() { IsPresentation = false });
                prms.Algorithms.Add(new BestFitDecreasing() { IsPresentation = false });

                prms.Distributions.Add(Distribution.Uniform);

                bw.RunWorkerAsync(prms);
            }
        }
    }
}
