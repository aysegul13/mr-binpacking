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
                newBin.ShowFiller = false;
                newBin.ShowAsElement = true;
                newBin.Bin = bin;

                newBin.Border.BorderThickness = new Thickness(0);
                newBin.Border.Background = Brushes.Transparent;
                newBin.laFreeSpace.Visibility = Visibility.Collapsed;
                newBin.ShowFiller = false;

                spPreview.Children.Add(newBin);
            }

            int LB = Bounds.LowerBound(Elements, BinSize);
            int SLB = Bounds.StrongerLowerBound(Elements, BinSize, BinSize / 2 - 1);

            laBounds.Visibility = Visibility.Visible;
            laBounds.Content = String.Format("Dolne ograniczenia LB/SLB: {0}/{1}", LB, SLB);
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

        ExperimentInstance Instance = null;

        private void bExperiment_Click(object sender, RoutedEventArgs e)
        {
            ExperimentParams expParams = new ExperimentParams(GetExpParamsFromUI());
            ExperimentProgressWindow expWindow;

            if (rbSourceGenerator.IsChecked == true)
                expWindow = new ExperimentProgressWindow(expParams, null);
            else
                expWindow = new ExperimentProgressWindow(expParams, Instance);

            ChildWindows.Add(expWindow);
            expWindow.Show();
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
            List<BaseAlgorithm> algorithms = new List<BaseAlgorithm>();
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
            if (cbAAS.IsChecked == true)
            {
                double epsilon = Double.Parse(ntbAASEpsilon.Text);
                algorithms.Add(new AAS() { Epsilon = epsilon });
            }

            foreach (var alg in algorithms)
            {
                if (alg is ListAlgorithm)
                {
                    (alg as ListAlgorithm).IsPresentation = false;
                    (alg as ListAlgorithm).IsWaiting = false;
                }

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
            ntbAASEpsilon.Text = experimentParams.AASEpsilon.ToString();

            cbExpDistUniform.IsChecked = experimentParams.Distributions.Contains(Distribution.Uniform);
            cbExpDistGauss.IsChecked = experimentParams.Distributions.Contains(Distribution.Gauss);
            cbExpDistExp.IsChecked = experimentParams.Distributions.Contains(Distribution.Exponential);

            cbExpSortNone.IsChecked = experimentParams.Sortings.Contains(Sorting.None);
            cbExpSortAsc.IsChecked = experimentParams.Sortings.Contains(Sorting.Ascending);
            cbExpSortDesc.IsChecked = experimentParams.Sortings.Contains(Sorting.Descending);

            cbExpNF.IsChecked = experimentParams.Algorithms.Contains(Algorithm.NextFit);
            cbExpFF.IsChecked = experimentParams.Algorithms.Contains(Algorithm.FirstFit);
            cbExpBF.IsChecked = experimentParams.Algorithms.Contains(Algorithm.BestFit);
            cbExpFFD.IsChecked = experimentParams.Algorithms.Contains(Algorithm.FirstFitDecreasing);
            cbExpBFD.IsChecked = experimentParams.Algorithms.Contains(Algorithm.BestFitDecreasing);
            cbExpRF.IsChecked = experimentParams.Algorithms.Contains(Algorithm.RandomFit);
            cbExpAAS.IsChecked = experimentParams.Algorithms.Contains(Algorithm.AsymptoticApproximationScheme);
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
            if (cbExpDistUniform.IsChecked == true)
                distributions.Add(Distribution.Uniform);
            if (distributions.Count == 0)
            {
                distributions.Add(Distribution.Uniform);
                cbExpDistUniform.IsChecked = true;
            }
            experimentParams.Distributions = distributions;

            //"None" must be first!!!
            if (cbExpSortNone.IsChecked == true)
                experimentParams.Sortings.Add(Sorting.None);
            if (cbExpSortAsc.IsChecked == true)
                experimentParams.Sortings.Add(Sorting.Ascending);
            if (cbExpSortDesc.IsChecked == true)
                experimentParams.Sortings.Add(Sorting.Descending);
            if (experimentParams.Sortings.Count == 0)
            {
                experimentParams.Sortings.Add(Sorting.None);
                cbExpSortNone.IsChecked = true;
            }

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
            if (cbExpAAS.IsChecked == true)
            {
                experimentParams.AASEpsilon = Double.Parse(ntbAASExpEpsilon.Text);
                algorithms.Add(Algorithm.AsymptoticApproximationScheme);
            }
            if (algorithms.Count == 0)
            {
                algorithms.Add(Algorithm.NextFit);
                cbExpNF.IsChecked = true;
            }

            experimentParams.Algorithms = algorithms;

            return experimentParams;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var childWindow in ChildWindows)
            {
                if (childWindow.IsVisible)
                    childWindow.Close();
            }
        }

        private void ntbBinSize_LostFocus(object sender, RoutedEventArgs e)
        {
            ntbMinValue.MaxValue = ntbMaxValue.MaxValue = Int32.Parse(ntbBinSize.Text);
        }

        private void ntbMinValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Int32.Parse(ntbMinValue.Text) > Int32.Parse(ntbMaxValue.Text))
                ntbMinValue.Text = ntbMaxValue.Text;
        }

        private void ntbMaxValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (Int32.Parse(ntbMaxValue.Text) < Int32.Parse(ntbMinValue.Text))
                ntbMaxValue.Text = ntbMinValue.Text;
        }

        private void tbElements_LostFocus(object sender, RoutedEventArgs e)
        {
            BinSize = Int32.Parse(ntbBinSize.Text);
            int parse;

            Elements = (from elemStr in tbElements.Text.Split()
                        where (Int32.TryParse(elemStr, out parse) && (Int32.Parse(elemStr) < BinSize))
                        select Int32.Parse(elemStr)).ToList();

            RefreshElements();
            RefreshPreview();
        }

        private void cbAAS_CheckedChanged(object sender, RoutedEventArgs e)
        {
            bPresentation.IsEnabled = (bool)!cbAAS.IsChecked;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BinSize = Int32.Parse(ntbBinSize.Text);
            int elementsNumber = Int32.Parse(ntbElementsNumber.Text);
            int minValue = Int32.Parse(ntbMinValue.Text);
            int maxValue = Int32.Parse(ntbMaxValue.Text);

            Elements = Generator.GenerateData(elementsNumber, minValue, maxValue, Distribution.Uniform);

            RefreshElements();
            RefreshPreview();
        }

        private void bLoad3_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == true)
            {
                ESLoader.LoadFromFile2(openDialog.FileName);
                //ExperimentParamsFile experimentParams = Loader.LoadExperimentParams(openDialog.FileName);
                //SetParamsToUI(experimentParams);
            }
        }

        private void bSavePreview_Click(object sender, RoutedEventArgs e)
        {
            Loader.SaveToImg(spPreview, spPreview.ActualWidth, spPreview.ActualHeight);
        }

        private void bLoadGenSettings_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == true)
            {
                ExperimentParamsFile experimentParams = Loader.LoadExperimentParams(openDialog.FileName);
                SetParamsToUI(experimentParams);
            }
        }

        private void bSaveGenSettings_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            if (saveDialog.ShowDialog() == true)
            {
                ExperimentParamsFile experimentParams = GetExpParamsFromUI();
                Loader.SaveExperimentParams(experimentParams, saveDialog.FileName);
            }
        }

        private void bLoadExpInstances_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == true)
            {
                FileTypeWindow fileTypeWindow = new FileTypeWindow(openDialog.FileName);
                fileTypeWindow.ShowDialog();

                Instance = fileTypeWindow.Result;

                //if (result != null)
                //    Instances = fileTypeWindow.Result.Select(inst => 
                //else
                //    Instances = null;

                //ESLoader.LoadFromFile2(openDialog.FileName);
                //ExperimentParamsFile experimentParams = Loader.LoadExperimentParams(openDialog.FileName);
                //SetParamsToUI(experimentParams);
            }
        }
    }
}
