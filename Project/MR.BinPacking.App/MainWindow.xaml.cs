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
using MR.BinPacking.App.Properties;
using System.Reflection;
using System.Threading;

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

        public static void ShowError(Exception exc, string methodName)
        {
            MessageBox.Show(exc.Message + exc.StackTrace, methodName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void bUniformDist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BinSize = Int32.Parse(ntbBinSize.Text);
                int elementsNumber = Int32.Parse(ntbElementsNumber.Text);
                int minValue = Int32.Parse(ntbMinValue.Text);
                int maxValue = Int32.Parse(ntbMaxValue.Text);

                Elements = Generator.GenerateData(elementsNumber, minValue, maxValue, Distribution.Uniform);
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }

            RefreshElements();
            RefreshPreview();
        }

        private void bGaussDist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BinSize = Int32.Parse(ntbBinSize.Text);
                int elementsNumber = Int32.Parse(ntbElementsNumber.Text);
                int minValue = Int32.Parse(ntbMinValue.Text);
                int maxValue = Int32.Parse(ntbMaxValue.Text);

                Elements = Generator.GenerateData(elementsNumber, minValue, maxValue, Distribution.Gauss);
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }

            RefreshElements();
            RefreshPreview();
        }

        private void bExponentialDist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BinSize = Int32.Parse(ntbBinSize.Text);
                int elementsNumber = Int32.Parse(ntbElementsNumber.Text);
                int minValue = Int32.Parse(ntbMinValue.Text);
                int maxValue = Int32.Parse(ntbMaxValue.Text);

                Elements = Generator.GenerateData(elementsNumber, minValue, maxValue, Distribution.Exponential);
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }

            RefreshElements();
            RefreshPreview();
        }

        private void RefreshPreview()
        {
            try
            {
                spPreview.Children.Clear();

                foreach (var elem in Elements)
                {
                    Bin bin = new Bin(BinSize);
                    bin.Insert(elem);

                    BinControl newBin = new BinControl();
                    newBin.ShowScaled = Settings.Default.PRE_ScaleElements;
                    newBin.ShowFiller = false;
                    newBin.ShowAsElement = true;
                    newBin.Bin = bin;

                    newBin.Border.BorderThickness = new Thickness(0);
                    newBin.Border.Background = Brushes.Transparent;
                    newBin.Border.BorderBrush = BinControl.borderBrush;
                    newBin.laFreeSpace.Visibility = Visibility.Collapsed;
                    newBin.ShowFiller = false;

                    spPreview.Children.Add(newBin);
                }

                int L1 = Bounds.L1(Elements, BinSize);
                int L2 = Bounds.L2(Elements, BinSize);

                laBounds.Visibility = Visibility.Visible;
                laBounds.Content = String.Format("Dolne ograniczenia L1/L2: {0}/{1}", L1, L2);
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void RefreshElements()
        {
            try
            {
                tbElements.Text = "";

                foreach (var elem in Elements)
                    tbElements.Text += elem + " ";
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void bShuffle_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Elements = this.Elements.OrderBy(el => Guid.NewGuid()).ToList();
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }

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

        ExpInstance Instance = null;

        private void bExperiment_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ExpParams expParams = new ExpParams(GetExpParamsFromUI());
                ExperimentProgressWindow expWindow;

                if (rbSourceGenerator.IsChecked == true)
                    expWindow = new ExperimentProgressWindow(expParams, null);
                else
                    expWindow = new ExperimentProgressWindow(expParams, Instance);

                ChildWindows.Add(expWindow);
                expWindow.Show();
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void bLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog()
                {
                    DefaultExt = ".bpp",
                    Filter = "Plik z instancjami problemu pakowania (*.bpp)|*.bpp|Wszystkie pliki (*.*)|*.*",
                    FilterIndex = 1
                };

                if (openDialog.ShowDialog() == true)
                {
                    FileTypeWindow fileTypeWindow = new FileTypeWindow(openDialog.FileName);
                    fileTypeWindow.ShowDialog();

                    Instance instance = fileTypeWindow.Result;
                    if (instance != null)
                    {
                        BinSize = instance.BinSize;
                        Elements = instance.Elements;

                        RefreshElements();
                        RefreshPreview();
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void bSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog()
                {
                    DefaultExt = ".bpp",
                    Filter = "Plik z instancjami problemu pakowania (*.bpp)|*.bpp|Wszystkie pliki (*.*)|*.*",
                    FilterIndex = 1
                };

                if (saveDialog.ShowDialog() == true)
                {
                    Instance instance = new Instance(BinSize)
                    {
                        Elements = this.Elements
                    };

                    Loader.SaveInstance(instance, saveDialog.FileName);
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void bPresentation_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void bResult_Click(object sender, RoutedEventArgs e)
        {
            try
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
                if (cbReduction.IsChecked == true)
                    algorithms.Add(new Reduction());
                if (cbExact.IsChecked == true)
                    algorithms.Add(new Exact());
                if (cbPBI.IsChecked == true)
                    algorithms.Add(new PBI());

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
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }


        private void SetUIFromExpParams(ExpParamsFile experimentParams)
        {
            try
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
                cbExpReduction.IsChecked = experimentParams.Algorithms.Contains(Algorithm.Reduction);
                cbExpExact.IsChecked = experimentParams.Algorithms.Contains(Algorithm.Exact);
                cbExpPBI.IsChecked = experimentParams.Algorithms.Contains(Algorithm.PBI);
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private ExpParamsFile GetExpParamsFromUI()
        {
            try
            {
                ExpParamsFile experimentParams = new ExpParamsFile()
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
                if (cbExpReduction.IsChecked == true)
                    algorithms.Add(Algorithm.Reduction);
                if (cbExpExact.IsChecked == true)
                    algorithms.Add(Algorithm.Exact);
                if (cbExpPBI.IsChecked == true)
                    algorithms.Add(Algorithm.PBI);

                if (algorithms.Count == 0)
                {
                    algorithms.Add(Algorithm.NextFit);
                    cbExpNF.IsChecked = true;
                }

                experimentParams.Algorithms = algorithms;

                return experimentParams;
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }

            return null;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                foreach (var childWindow in ChildWindows)
                {
                    if (childWindow.IsVisible)
                        childWindow.Close();
                }

                Settings.Default.VIS_MinValue = ntbMinValue.Text;
                Settings.Default.VIS_MaxValue = ntbMaxValue.Text;
                Settings.Default.EXP_From = ntbExpMinN.Text;
                Settings.Default.EXP_To = ntbExpMaxN.Text;
                Settings.Default.EXP_MinValue = ntbExpMinVal.Text;
                Settings.Default.EXP_MaxValue = ntbExpMaxVal.Text;

                Settings.Default.Save();
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void ntbBinSize_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int binSize = Int32.Parse(ntbBinSize.Text);
                ntbMaxValue.MaxValue = binSize;

                int maxVal = Int32.Parse(ntbMaxValue.Text);
                ntbMinValue.MaxValue = maxVal;

                int minVal = Int32.Parse(ntbMinValue.Text);
                ntbMaxValue.MinValue = minVal;

                BinSize = Int32.Parse(ntbBinSize.Text);
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }

            RefreshElements();
            RefreshPreview();
        }

        private void ntbMinValue_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int minVal = Int32.Parse(ntbMinValue.Text);
                ntbMaxValue.MinValue = minVal;
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void ntbMaxValue_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int maxVal = Int32.Parse(ntbMaxValue.Text);
                ntbMinValue.MaxValue = maxVal;
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void tbElements_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int parse;
                Elements = (from elemStr in tbElements.Text.Split()
                            where Int32.TryParse(elemStr, out parse)
                            select Int32.Parse(elemStr)).ToList();
                //BinSize = Elements.Max();
                BinSize = Int32.Parse(ntbBinSize.Text);
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }

            RefreshElements();
            RefreshPreview();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                cbNonListAlgorithm_CheckedChanged(null, null);

                BinSize = Int32.Parse(ntbBinSize.Text);
                int elementsNumber = Int32.Parse(ntbElementsNumber.Text);
                int minValue = Int32.Parse(ntbMinValue.Text);
                int maxValue = Int32.Parse(ntbMaxValue.Text);

                Elements = Generator.GenerateData(elementsNumber, minValue, maxValue, Distribution.Uniform);
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }

            RefreshElements();
            RefreshPreview();
        }

        private void bSavePreview_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Loader.SaveControlImage(spPreview, spPreview.ActualWidth, spPreview.ActualHeight);
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void bLoadGenSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog();
                if (openDialog.ShowDialog() == true)
                {
                    ExpParamsFile experimentParams = Loader.LoadExperimentParams(openDialog.FileName);
                    SetUIFromExpParams(experimentParams);
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void bSaveGenSettings_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveDialog = new SaveFileDialog();
                if (saveDialog.ShowDialog() == true)
                {
                    ExpParamsFile experimentParams = GetExpParamsFromUI();
                    Loader.SaveExperimentParams(experimentParams, saveDialog.FileName);
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void bLoadExpInstances_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openDialog = new OpenFileDialog()
                {
                    DefaultExt = ".bpp",
                    Filter = "Plik z instancjami problemu pakowania (*.bpp)|*.bpp|Wszystkie pliki (*.*)|*.*",
                    FilterIndex = 1
                };

                if (openDialog.ShowDialog() == true)
                {
                    FileTypeWindow fileTypeWindow = new FileTypeWindow(openDialog.FileName);
                    fileTypeWindow.ShowDialog();

                    Instance = fileTypeWindow.Result;
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void cbNonListAlgorithm_CheckedChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                if (bPresentation == null)
                    return;

                bPresentation.IsEnabled = !((bool)cbAAS.IsChecked
                    || (bool)cbReduction.IsChecked
                    || (bool)cbExact.IsChecked
                    || (bool)cbPBI.IsChecked);
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void cbScaleElements_CheckedChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (BinControl binControl in spPreview.Children)
                    binControl.ShowScaled = Settings.Default.PRE_ScaleElements;
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void ntbExpMinN_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int minVal = Int32.Parse(ntbExpMinN.Text);
                ntbExpMaxN.MinValue = minVal;
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void ntbExpMaxN_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                int maxVal = Int32.Parse(ntbExpMaxN.Text);
                ntbExpMinN.MaxValue = maxVal;
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void ntbExpMinVal_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                double minVal = Double.Parse(ntbExpMinVal.Text);
                ntbExpMaxVal.MinValue = minVal;
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void ntbExpMaxVal_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                double maxVal = Double.Parse(ntbExpMaxVal.Text);
                ntbExpMinVal.MaxValue = maxVal;
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
