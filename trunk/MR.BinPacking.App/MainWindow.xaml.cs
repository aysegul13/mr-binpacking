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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using MR.BinPacking.App.Controls;
using MR.BinPacking.Library.Base;
using MR.BinPacking.Library;
using MR.BinPacking.App.Utils;
using System.Windows.Interop;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using MR.BinPacking.Library.Algorithms;
using MR.BinPacking.Library.Utils;
using MR.BinPacking.Library.Experiment;

namespace MR.BinPacking.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<int> Elements = new List<int>();
        int BinSize = 10;
        List<Window> ChildWindows = new List<Window>();

        private void bUniformDist_Click(object sender, RoutedEventArgs e)
        {
            BinSize = Int32.Parse(ntbBinSize.Text);
            int elementsNumber = Int32.Parse(ntbElementsNumber.Text);
            int minValue = Int32.Parse(ntbMinValue.Text);
            int maxValue = Int32.Parse(ntbMaxValue.Text);

            Elements = Generator.GenerateData(elementsNumber, minValue, maxValue, Distribution.Uniform);

            RefreshElements();
            RefreshPreview();
        }

        private void bGaussDist_Click(object sender, RoutedEventArgs e)
        {
            BinSize = Int32.Parse(ntbBinSize.Text);
            int elementsNumber = Int32.Parse(ntbElementsNumber.Text);
            int minValue = Int32.Parse(ntbMinValue.Text);
            int maxValue = Int32.Parse(ntbMaxValue.Text);

            Elements = Generator.GenerateData(elementsNumber, minValue, maxValue, Distribution.Gauss);

            RefreshElements();
            RefreshPreview();
        }

        private void bExponentialDist_Click(object sender, RoutedEventArgs e)
        {
            BinSize = Int32.Parse(ntbBinSize.Text);
            int elementsNumber = Int32.Parse(ntbElementsNumber.Text);
            int minValue = Int32.Parse(ntbMinValue.Text);
            int maxValue = Int32.Parse(ntbMaxValue.Text);

            Elements = Generator.GenerateData(elementsNumber, minValue, maxValue, Distribution.Exponential);

            RefreshElements();
            RefreshPreview();
        }

        private void RefreshPreview()
        {
            spPreview.Children.Clear();

            foreach (var elem in Elements)
            {
                Bin bin = new Bin(BinSize);
                bin.Insert(elem);

                BinControl newBin = new BinControl();
                newBin.AutoRefresh = false;
                newBin.Bin = bin;
                newBin.bFiller.Visibility = Visibility.Collapsed;

                newBin.Border.BorderThickness = new Thickness(0);
                newBin.Border.Background = Brushes.Transparent;
                newBin.laFreeSpace.Visibility = Visibility.Collapsed;
                newBin.ShowFiller = false;

                foreach (var elemControl in newBin.DataItems)
                    elemControl.Border.BorderThickness = new Thickness(3);

                spPreview.Children.Add(newBin);
            }
        }

        private void RefreshElements()
        {
            tbElements.Text = "";

            foreach (var elem in Elements)
                tbElements.Text += elem + " ";
        }

        private void bShuffle_Click(object sender, RoutedEventArgs e)
        {
            this.Elements = this.Elements.OrderBy(el => Guid.NewGuid()).ToList();

            RefreshElements();
            RefreshPreview();
        }

        private void bSortAsc_Click(object sender, RoutedEventArgs e)
        {
            Elements.Sort();
            RefreshElements();
            RefreshPreview();
        }

        private void bSortDesc_Click(object sender, RoutedEventArgs e)
        {
            Elements.Sort((x, y) => y.CompareTo(x));
            RefreshElements();
            RefreshPreview();
        }

        private void bExperiment_Click(object sender, RoutedEventArgs e)
        {
            //List<ListAlgorithm> algorithms = new List<ListAlgorithm>() { new NextFit(), new FirstFit() };

            //ExperimentParams prms = new ExperimentParams()
            //{
            //    Algorithms = algorithms,
            //    BinSize = 100,
            //    Dist = Distribution.Uniform,
            //    MinN = 100,
            //    MaxN = 1000,
            //    Step = 100,
            //    MinVal = 0.0,
            //    MaxVal = 1.0,
            //    Repeat = 2
            //};

            //TestWindow test = new TestWindow(null);
            //test.Show();

            ExperimentProgressWindow test = new ExperimentProgressWindow(null);
            ChildWindows.Add(test);
            test.Show();
        }

        private void bLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == true)
            {
                Instance instance = Loader.LoadFromFile(openDialog.FileName);
                BinSize = instance.BinSize;
                Elements = instance.Elements;


                RefreshElements();
                RefreshPreview();
            }
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            if (saveDialog.ShowDialog() == true)
            {
                Instance instance = new Instance(BinSize)
                {
                    Elements = this.Elements
                };

                Loader.SaveToFile(instance, saveDialog.FileName);
            }
        }

        private void bPresentation_Click(object sender, RoutedEventArgs e)
        {
            List<ListAlgorithm> algorithms = new List<ListAlgorithm>();
            if (cbNextFit.IsChecked == true)
                algorithms.Add(new NextFit());
            if (cbFirstFit.IsChecked == true)
                algorithms.Add(new FirstFit());
            if (cbBestFit.IsChecked == true)
                algorithms.Add(new BestFit());
            if (cbFirstFitD.IsChecked == true)
                algorithms.Add(new FirstFitDecreasing());
            if (cbBestFitD.IsChecked == true)
                algorithms.Add(new BestFitDecreasing());
            if (cbRandomFit.IsChecked == true)
                algorithms.Add(new RandomFit());

            foreach (var alg in algorithms)
            {
                PreviewWindow prev = new PreviewWindow(alg, Elements, BinSize);
                ChildWindows.Add(prev);
                prev.Show();
            }
        }

        private void bResult_Click(object sender, RoutedEventArgs e)
        {
            List<ListAlgorithm> algorithms = new List<ListAlgorithm>();
            if (cbNextFit.IsChecked == true)
                algorithms.Add(new NextFit());
            if (cbFirstFit.IsChecked == true)
                algorithms.Add(new FirstFit());
            if (cbBestFit.IsChecked == true)
                algorithms.Add(new BestFit());
            if (cbFirstFitD.IsChecked == true)
                algorithms.Add(new FirstFitDecreasing());
            if (cbBestFitD.IsChecked == true)
                algorithms.Add(new BestFitDecreasing());
            if (cbRandomFit.IsChecked == true)
                algorithms.Add(new RandomFit());

            foreach (var alg in algorithms)
            {
                alg.IsPresentation = false;
                alg.IsWaiting = false;
                PreviewWindow prev = new PreviewWindow(alg, Elements, BinSize);
                ChildWindows.Add(prev);
                prev.Show();
            }
        }

        private void SetParamsToUI(ExperimentParamsFile experimentParams)
        {
            ntbExpMinN.Text = experimentParams.MinN.ToString();
            ntbExpMaxN.Text = experimentParams.MaxN.ToString();
            ntbExpStep.Text = experimentParams.Step.ToString();
            ntbExpBinSize.Text = experimentParams.BinSize.ToString();
            ntbExpMinVal.Text = experimentParams.MinVal.ToString();
            ntbExpMaxVal.Text = experimentParams.MaxVal.ToString();
            ntbExpRepeat.Text = experimentParams.Repeat.ToString();

            cbExpDistUniform.IsChecked = experimentParams.Distributions.Contains(Distribution.Uniform);
            cbExpDistGauss.IsChecked = experimentParams.Distributions.Contains(Distribution.Gauss);
            cbExpDistExp.IsChecked = experimentParams.Distributions.Contains(Distribution.Exponential);

            rbExpSortNone.IsChecked = experimentParams.Sortings.Contains(Sorting.None);
            rbExpSortAsc.IsChecked = experimentParams.Sortings.Contains(Sorting.Ascending);
            rbExpSortDesc.IsChecked = experimentParams.Sortings.Contains(Sorting.Descending);

            cbExpNF.IsChecked = experimentParams.Algorithms.Contains(Algorithm.NextFit);
            cbExpFF.IsChecked = experimentParams.Algorithms.Contains(Algorithm.FirstFit);
            cbExpBF.IsChecked = experimentParams.Algorithms.Contains(Algorithm.BestFit);
            cbExpFFD.IsChecked = experimentParams.Algorithms.Contains(Algorithm.FirstFitDecreasing);
            cbExpBFD.IsChecked = experimentParams.Algorithms.Contains(Algorithm.BestFitDecreasing);
            cbExpRF.IsChecked = experimentParams.Algorithms.Contains(Algorithm.RandomFit);
        }

        private void bExpLoadParams_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == true)
            {
                ExperimentParamsFile experimentParams = Loader.LoadExperimentParams(openDialog.FileName);
                SetParamsToUI(experimentParams);
            }
        }

        private ExperimentParamsFile GetExpParamsFromUI()
        {
            ExperimentParamsFile experimentParams = new ExperimentParamsFile()
            {
                MinN = Int32.Parse(ntbExpMinN.Text),
                MaxN = Int32.Parse(ntbExpMaxN.Text),
                Step = Int32.Parse(ntbExpStep.Text),
                BinSize = Int32.Parse(ntbExpBinSize.Text),
                MinVal = Double.Parse(ntbExpMinVal.Text),
                MaxVal = Double.Parse(ntbExpMaxVal.Text),
                Repeat = Int32.Parse(ntbExpRepeat.Text)
            };

            List<Distribution> distributions = new List<Distribution>();
            if (cbExpDistGauss.IsChecked == true)
                distributions.Add(Distribution.Gauss);
            if (cbExpDistExp.IsChecked == true)
                distributions.Add(Distribution.Exponential);
            if ((cbExpDistUniform.IsChecked == true) || (distributions.Count == 0))
                distributions.Add(Distribution.Uniform);
            experimentParams.Distributions = distributions;

            //"None" must be first!!!
            if (rbExpSortNone.IsChecked == true)
                experimentParams.Sortings.Add(Sorting.None);
            if (rbExpSortAsc.IsChecked == true)
                experimentParams.Sortings.Add(Sorting.Ascending);
            if (rbExpSortDesc.IsChecked == true)
                experimentParams.Sortings.Add(Sorting.Descending);

            List<Algorithm> algorithms = new List<Algorithm>();
            if (cbExpNF.IsChecked == true)
                algorithms.Add(Algorithm.NextFit);
            if (cbExpFF.IsChecked == true)
                algorithms.Add(Algorithm.FirstFit);
            if (cbExpBF.IsChecked == true)
                algorithms.Add(Algorithm.BestFit);
            if (cbExpFFD.IsChecked == true)
                algorithms.Add(Algorithm.FirstFitDecreasing);
            if (cbExpBFD.IsChecked == true)
                algorithms.Add(Algorithm.BestFitDecreasing);
            if (cbExpRF.IsChecked == true)
                algorithms.Add(Algorithm.RandomFit);
            experimentParams.Algorithms = algorithms;

            return experimentParams;
        }

        private void bExpSaveParams_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            if (saveDialog.ShowDialog() == true)
            {
                ExperimentParamsFile experimentParams = GetExpParamsFromUI();
                Loader.SaveExperimentParams(experimentParams, saveDialog.FileName);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var childWindow in ChildWindows)
            {
                if (childWindow.IsVisible)
                    childWindow.Close();
            }
        }
    }
}
