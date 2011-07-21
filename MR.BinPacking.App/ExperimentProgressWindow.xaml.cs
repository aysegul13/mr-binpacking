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
using System.IO;
using Microsoft.Win32;

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
                    int min = (int)Math.Ceiling(prms.MinVal * N);
                    int max = (int)Math.Ceiling(prms.MaxVal * N);

                    foreach (var D in prms.Distributions)
                    {
                        List<int> elements = Generator.GenerateData(N, min, max, D);
                        foreach (var S in prms.Sortings)
                        {
                            elements = Experiment.GetElementsWithSorting(elements, S);

                            //TODO: algorytm z innego źródła
                            for (int i = 0; i < prms.Algorithms.Count; i++)
                            {
                                ListAlgorithm A = prms.Algs[i];
                                ExperimentState state = new ExperimentState()
                                {
                                    Repeat = R,
                                    N = N,
                                    Distribution = D,
                                    Sorting = S,
                                    AlgorithmName = A.Name,
                                    Samples = samplesCount,
                                    ActualSample = sampleNumber + 1
                                };
                                worker.ReportProgress(0, state);

                                if (worker.CancellationPending)
                                {
                                    e.Cancel = true;
                                    return;
                                }

                                Stopwatch sw = new Stopwatch();
                                sw.Start();
                                Instance instanceResult = A.Execute(elements, prms.BinSize);
                                sw.Stop();

                                //TODO: dopisać info o algorytmie
                                Sample stats = new Sample()
                                {
                                    ID = sampleNumber,
                                    N = N,
                                    Distribution = D,
                                    Sorting = S,
                                    Algorithm = prms.Algorithms[i],
                                    LowerBound = Bounds.LowerBound(elements, prms.BinSize),
                                    StrongerLowerBound = Bounds.StrongerLowerBound(elements, prms.BinSize, prms.BinSize / 2 - 1),
                                    Result = instanceResult.Bins.Count(),
                                    ExecutionTime = sw.ElapsedMilliseconds
                                };

                                result.Samples.Add(stats);
                                sampleNumber++;
                            }
                        }
                    }

                    N += prms.Step;
                }
            }

            e.Result = result;
        }

        void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ExperimentState state = e.UserState as ExperimentState;

            laRepeat.Content = String.Format("Powtórzenie: {0}", state.Repeat + 1);
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
                //result.GetDataSeries();


                guiChart.DataSource = result;

                List<DataSerie> dataSeries = result.GetDataSeries(ChartDataType.AlgorithmDistribution, StatField.ExecutionTime);
                //guiChart.DataSeries = dataSeries;

                ////for (int i = 0; i < stats.DataSeries.Count; i++)
                ////    guiChart.DataSeries.Add(Experiment.GetCoordinates(stats[i], StatField.ExecutionTime));

                //guiChart.AxisXIntervalWidth = result.Params.Step;
                //guiChart.AxisXMin = result.Params.MinN;
                //guiChart.Refresh();

                res = dataSeries;
                expRes = result;
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
                    Algs = new List<ListAlgorithm>(),
                    Algorithms = new List<Algorithm>(),
                    BinSize = 100,
                    Distributions = new List<Distribution>(),
                    Sortings = new List<Sorting>(),
                    MinN = 500,
                    MaxN = 1000,
                    Step = 100,
                    MinVal = 0.0,
                    MaxVal = 1.0,
                    Repeat = 1
                };

                prms.Algorithms.Add(Algorithm.NextFit);
                prms.Algorithms.Add(Algorithm.FirstFit);
                //prms.Algorithms.Add(Algorithm.BestFit);
                prms.Algorithms.Add(Algorithm.BestFitDecreasing);

                prms.Algs.Add(new NextFit() { IsPresentation = false });
                prms.Algs.Add(new FirstFit() { IsPresentation = false });
                //prms.Algs.Add(new BestFit() { IsPresentation = false });
                prms.Algs.Add(new BestFitDecreasing() { IsPresentation = false });

                prms.Distributions.Add(Distribution.Uniform);
                prms.Distributions.Add(Distribution.Gauss);

                prms.Sortings.Add(Sorting.None);
                prms.Sortings.Add(Sorting.Ascending);

                bw.RunWorkerAsync(prms);
            }
        }

        private string GetTableName(ChartDataType type, StatField field)
        {
            string name = "";
            switch (type)
            {
                case ChartDataType.Distribution:
                    name = "PORÓWNANIE ROZKŁADÓW";
                    break;
                case ChartDataType.Sorting:
                    name = "PORÓWNANIE SORTOWANIA";
                    break;
                case ChartDataType.AlgorithmDistribution:
                    name = "PORÓWNANIE ALGORYTMÓW I ROZKŁADÓW";
                    break;
                case ChartDataType.AlgorithmSorting:
                    name = "PORÓWNANIE ALGORYTMÓW I SORTOWANIA";
                    break;
                case ChartDataType.DistributionSorting:
                    name = "PORÓWNANIE ROZKŁADÓW I SORTOWANIA";
                    break;
                default:
                    name = "PORÓWNANIE ALGORYTMÓW";
                    break;
            }

            name += " - ";

            switch (field)
            {
                case StatField.LowerBound:
                    return name + "dolne ograniczenie [liczba elementów]";
                case StatField.StrongerLowerBound:
                    return name + "silniejsze dolne ograniczenie [liczba elementów]";
                case StatField.Result:
                    return name + "wynik [liczba elementów]";
                default:
                    return name + "czas działania [ms]";
            }
        }

        List<DataSerie> res;
        ExperimentResult expRes;
        private void bTable_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < res.Count + 1; i++)
            {
                gTable.RowDefinitions.Add(new RowDefinition());

                for (int j = 0; j < res[0].Points.Count + 1; j++)
                {
                    Border border = new Border()
                    {
                        BorderBrush = Brushes.Black
                    };

                    TextBlock textBlock = new TextBlock();
                    border.Child = textBlock;

                    if (i == 0)
                    {
                        gTable.ColumnDefinitions.Add(new ColumnDefinition());
                        border.BorderThickness = new Thickness(2.0);
                        textBlock.FontWeight = FontWeights.Bold;

                        if (j == 0)
                        {
                            gTable.ColumnDefinitions.Last().Width = new GridLength(1.0, GridUnitType.Auto);
                            textBlock.Text = "Seria danych \\ N";
                        }
                        else
                        {
                            int N = expRes.Params.MinN + (j - 1) * expRes.Params.Step;
                            textBlock.Text = N.ToString();
                        }
                    }
                    else
                    {
                        if (j == 0)
                        {
                            border.BorderThickness = new Thickness(2.0);
                            textBlock.FontWeight = FontWeights.Bold;
                            textBlock.Text = res[i - 1].Name;
                        }
                        else
                        {
                            border.BorderThickness = new Thickness(1.0);
                            textBlock.Text = res[i - 1].Points[j - 1].Y.ToString();
                        }
                    }

                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);
                    gTable.Children.Add(border);
                }
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            if (saveDialog.ShowDialog() == true)
                SaveResultsToFile(ChartDataType.AlgorithmDistribution, StatField.ExecutionTime, saveDialog.FileName);
        }

        public void SaveResultsToFile(ChartDataType type, StatField field, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine(GetTableName(type, field) + ";");

                for (int i = 0; i < res.Count + 1; i++)
                {
                    for (int j = 0; j < res[0].Points.Count; j++)
                    {
                        if (i == 0)
                        {
                            if (j == 0)
                            {
                                sw.Write("Seria danych \\ N;");
                            }
                            else
                            {
                                int N = expRes.Params.MinN + (j - 1) * expRes.Params.Step;
                                sw.Write(N + ";");
                            }
                        }
                        else
                        {
                            if (j == 0)
                                sw.Write(res[i - 1].Name + ";");
                            else
                                sw.Write(res[i - 1].Points[j - 1].Y + ";");
                        }
                    }

                    sw.WriteLine();
                }
            }
        }
    }
}
