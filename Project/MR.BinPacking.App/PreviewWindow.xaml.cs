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
using MR.BinPacking.Library.Base;
using MR.BinPacking.App.Controls;
using System.Diagnostics;
using MR.BinPacking.Library;
using System.Windows.Interop;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using MR.BinPacking.App.Utils;
using MR.BinPacking.App.Properties;
using System.Reflection;

namespace MR.BinPacking.App
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        private BaseAlgorithm Algorithm;
        private List<int> Elements;
        private int BinSize = 10;

        private List<BinControl> previewBins = new List<BinControl>();
        private List<BinControl> algorithmBins = new List<BinControl>();

        private Stopwatch stopWatch = new Stopwatch();

        public PreviewWindow(BaseAlgorithm algorithm, List<int> elements, int binSize)
        {
            InitializeComponent();

            Algorithm = algorithm;
            Elements = elements;
            BinSize = binSize;

            this.Title = algorithm.Name;
            showExecutionTime = (!(Algorithm is ListAlgorithm) || !(Algorithm as ListAlgorithm).IsPresentation);
        }


        public void DoWork()
        {
            try
            {
                stopWatch.Reset();
                stopWatch.Start();
                result = Algorithm.Execute(Elements, BinSize);
                stopWatch.Stop();

                this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        vbLoading.Visibility = Visibility.Collapsed;
                        Elements = Algorithm.Result.Elements;

                        DrawPreview();
                        Draw(result);
                        ShowInfo();
                    }));
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        public void Resume()
        {
            if (Algorithm is ListAlgorithm)
                (Algorithm as ListAlgorithm).IsWaiting = false;
        }

        private void DrawPreview()
        {
            try
            {
                spElements.Children.Clear();
                previewBins.Clear();

                for (int i = 0; i < Elements.Count; i++)
                {
                    int elem = Elements[i];
                    Bin bin = new Bin(BinSize);
                    bin.Insert(elem);

                    BinControl newBin = new BinControl();
                    newBin.ShowScaled = Settings.Default.PRE_ScaleElements;
                    newBin.ShowFiller = false;
                    newBin.ShowAsElement = true;
                    newBin.Bin = bin;

                    newBin.LayoutTransform = new ScaleTransform(0.8, 0.8);
                    newBin.Border.BorderThickness = new Thickness(0);
                    newBin.Border.Background = Brushes.Transparent;
                    newBin.laFreeSpace.Visibility = Visibility.Collapsed;

                    if (previewBins.Count < Elements.Count)
                    {
                        spElements.Children.Add(newBin);
                        previewBins.Add(newBin);
                    }
                    else
                    {
                        spElements.Children.RemoveAt(i);
                        spElements.Children.Insert(i, newBin);

                        previewBins[i] = newBin;
                    }
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void Draw(Instance instance)
        {
            try
            {
                spResult.Children.Clear();
                algorithmBins.Clear();

                foreach (var bin in instance.Bins)
                {
                    BinControl newBin = new BinControl();
                    newBin.ShowScaled = Settings.Default.PRE_ScaleElements;
                    newBin.Bin = bin;

                    spResult.Children.Add(newBin);
                    algorithmBins.Add(newBin);

                    newBin.UpdateGrid();
                }

                UpdateSelection();
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        void UpdateSelection()
        {
            try
            {
                if (!(Algorithm is ListAlgorithm))
                    return;

                ListAlgorithm listAlgorithm = Algorithm as ListAlgorithm;

                if (listAlgorithm.PrevSelectedElement >= 0)
                {
                    previewBins[listAlgorithm.PrevSelectedElement].StopAnimation();
                    previewBins[listAlgorithm.PrevSelectedElement].Border.Opacity = 0.5;
                }

                if (result == null)
                {
                    if (listAlgorithm.SelectedElement >= 0)
                        previewBins[listAlgorithm.SelectedElement].StartAnimation();

                    if (listAlgorithm.SelectedBin >= 0)
                        algorithmBins[listAlgorithm.SelectedBin].StartAnimation();
                }
                if (result != null)
                {
                    if (listAlgorithm.SelectedElement >= 0)
                        previewBins[listAlgorithm.SelectedElement].StopAnimation();

                    if (listAlgorithm.SelectedBin >= 0)
                        algorithmBins[listAlgorithm.SelectedBin].StopAnimation();
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }


        bool showExecutionTime = false;
        bool first = true;
        Instance result = null;
        Thread workerThread = null;
        private void Execute()
        {
            try
            {
                workerThread = new Thread(this.DoWork);
                workerThread.Start();

                //while (!workerThread.IsAlive) ;
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void bNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Algorithm is ListAlgorithm)
                {
                    tblMessage.Text = (Algorithm as ListAlgorithm).Message;
                    (Algorithm as ListAlgorithm).ShowScaled = Settings.Default.PRE_ScaleElements;
                }

                if (result == null)
                {
                    Elements = Algorithm.Result.Elements;

                    Instance inst = new Instance(Algorithm.Result.BinSize);
                    foreach (var bin in Algorithm.Result.Bins)
                    {
                        Bin newBin = new Bin(Algorithm.Result.BinSize);
                        newBin.Elements.AddRange(bin.Elements);
                        inst.Bins.Add(newBin);
                    }

                    if (first)
                        DrawPreview();
                    first = false;
                    Draw(inst);
                    Resume();
                }
                else
                {
                    bNext.IsEnabled = false;
                }
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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

        private void bEnd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                showExecutionTime = false;

                if (Algorithm is ListAlgorithm)
                    (Algorithm as ListAlgorithm).IsPresentation = false;

                Resume();
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void UpdateOpacity()
        {
            try
            {
                foreach (var bin in previewBins)
                    bin.Border.Opacity = 1.0;
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void ShowInfo()
        {
            try
            {
                UpdateOpacity();

                laBinCount.Visibility = Visibility.Visible;
                laLowerBounds.Visibility = Visibility.Visible;
                laQualityEstimations.Visibility = Visibility.Visible;
                laErrorEstimations.Visibility = Visibility.Visible;

                if (showExecutionTime)
                    laExecutionTime.Visibility = Visibility.Visible;

                tblMessage.Visibility = Visibility.Collapsed;
                bNext.Visibility = Visibility.Collapsed;
                bEnd.Visibility = Visibility.Collapsed;


                laBinCount.Content = "Liczba pudełek: " + result.Bins.Count;
                laExecutionTime.Content = "Czas obliczeń [ms]: " + stopWatch.ElapsedMilliseconds;

                int L1 = Bounds.L1(Elements, BinSize);
                int L2 = Bounds.L2(Elements, BinSize);

                laLowerBounds.Content = String.Format("L1/L2: {0}/{1}", L1, L2);
                tblQualityEstimations.Text = String.Format("Oszac. jakości L1/L2: {0:0.00}/{1:0.00}",
                    (result.Bins.Count / (double)L1), (result.Bins.Count / (double)L2));

                tblErrorEstimations.Text = String.Format("Oszac. błędu L1/L2 [%]: {0:0.00}/{1:0.00}",
                    (100.0 * (result.Bins.Count - L1) / (double)L1),
                    (100.0 * (result.Bins.Count - L2) / (double)L2));
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void bSaveResult_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Loader.SaveControlImage(spResult, spResult.ActualWidth, spResult.ActualHeight);
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void bSaveElements_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Loader.SaveControlImage(spElements, spElements.ActualWidth, spElements.ActualHeight);
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                if ((Algorithm is ListAlgorithm) && (Algorithm as ListAlgorithm).IsPresentation)
                {
                    exPreview.IsExpanded = Settings.Default.PRE_ExpandPreviewInPresentation;
                    exSideBar.IsExpanded = Settings.Default.PRE_ExpandSideBarInPresentation;
                    vbLoading.Visibility = Visibility.Collapsed;
                    (Algorithm as ListAlgorithm).ShowScaled = Settings.Default.PRE_ScaleElements;

                    DrawPreview();
                }
                else
                {
                    exPreview.IsExpanded = Settings.Default.PRE_ExpandPreviewInResult;
                    exSideBar.IsExpanded = Settings.Default.PRE_ExpandSideBarInResult;
                    vbLoading.Visibility = Visibility.Visible;
                }

                Execute();
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
