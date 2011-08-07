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
        }


        public void DoWork()
        {
            stopWatch.Reset();
            stopWatch.Start();
            result = Algorithm.Execute(Elements, BinSize);
            stopWatch.Stop();
        }

        public void GoAhead()
        {
            if (Algorithm is ListAlgorithm)
                (Algorithm as ListAlgorithm).IsWaiting = false;
        }

        Instance result = null;

        private void DrawPreview()
        {
            spElements.Children.Clear();
            previewBins.Clear();

            for (int i = 0; i < Elements.Count; i++)
            {
                int elem = Elements[i];
                Bin bin = new Bin(BinSize);
                bin.Insert(elem);

                BinControl newBin = new BinControl();
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

        private void Draw(Instance instance)
        {
            spResult.Children.Clear();
            algorithmBins.Clear();

            foreach (var bin in instance.Bins)
            {
                BinControl newBin = new BinControl();
                newBin.Bin = bin;

                spResult.Children.Add(newBin);
                algorithmBins.Add(newBin);

                newBin.UpdateGrid();
            }

            UpdateSelection();
        }

        void UpdateSelection()
        {
            if (!(Algorithm is ListAlgorithm))
                return;

            ListAlgorithm listAlgorithm = Algorithm as ListAlgorithm;

            if (listAlgorithm.PrevSelectedElement >= 0)
            {
                previewBins[listAlgorithm.PrevSelectedElement].StopAnimation();
                previewBins[listAlgorithm.PrevSelectedElement].Border.Opacity = 0.5;
            }

            if (listAlgorithm.SelectedElement >= 0)
                previewBins[listAlgorithm.SelectedElement].StartAnimation();

            if (listAlgorithm.SelectedBin >= 0)
                algorithmBins[listAlgorithm.SelectedBin].StartAnimation();

            if (result != null)
            {
                previewBins[listAlgorithm.SelectedElement].StopAnimation();
                algorithmBins[listAlgorithm.SelectedBin].StopAnimation();
            }
        }

        private void Old()
        {
            //originalHeight = cnResult.ActualHeight;
            //cnResult.RenderTransform = new ScaleTransform(1.0, 3.0);

            Title = Algorithm.Name;


            Stopwatch sw = new Stopwatch();
            sw.Start();
            //Instance result = Algorithm.Execute(Elements, BinSize);
            sw.Stop();

            laBinCount.Content = "Liczba pudełek: " + result.Bins.Count;
            laExecutionTime.Content = "Czas obliczeń [ms]: " + sw.ElapsedMilliseconds;

            int minBound = Math.Min(Bounds.LowerBound(Elements, BinSize), Bounds.StrongerLowerBound(Elements, BinSize, BinSize / 2 - 1));
            //laQualityEstimation.Content = "Oszacowanie jakości: " + (result.Bins.Count / (double)minBound).ToString("0.000");
            //laErrorEstimation.Content = "Oszacowanie błędu: " + (100.0 * (result.Bins.Count - minBound) / (double)minBound).ToString("0.000");


            //laLowerBound.Content = "LB: " + Bounds.LowerBound(Elements, BinSize);
            //laStrongerLowerBound.Content = "SLB: " + Bounds.StrongerLowerBound(Elements, BinSize, BinSize / 2 - 1);


            sw.Reset();
            sw.Start();




            //cnResult.Children.Clear();
            //cnResult.Width = 32 * result.Bins.Count;


            //double totalHeight = Math.Max(100.0, cnResult.ActualHeight - 32 - 4);
            //int binCnt = 0;


            sw.Stop();

            //laDrawingTime.Content = "Czas rysowania: " + sw.ElapsedMilliseconds;
        }


        Thread workerThread = null;
        private void Execute()
        {
            workerThread = new Thread(this.DoWork);
            workerThread.Start();

            while (!workerThread.IsAlive) ;


            //this.Dispatcher.Invoke((Action)(() =>
            //{
            //    result = Algorithm.Execute(Elements, BinSize);
            //}));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if ((Algorithm is ListAlgorithm) && (Algorithm as ListAlgorithm).IsPresentation)
            {
                DrawPreview();
                Execute();
            }
            else
            {
                DoWork();
                Elements = Algorithm.Result.Elements;

                DrawPreview();
                Draw(result);
                ShowInfo();
            }
        }

        private bool first = true;
        private void bNext_Click(object sender, RoutedEventArgs e)
        {
            if (Algorithm is ListAlgorithm)
                tblMessage.Text = (Algorithm as ListAlgorithm).Message;

            if (result == null)
            {
                Elements = Algorithm.Result.Elements;

                Instance inst = new Instance(Algorithm.Result.BinSize);
                foreach (var bin in Algorithm.Result.Bins)
                {
                    Bin newBin = new Bin(Algorithm.Result.BinSize);

                    foreach (var elem in bin.Elements)
                        newBin.Insert(elem);

                    inst.Bins.Add(newBin);
                }

                if (first)
                    DrawPreview();
                first = false;
                Draw(inst);
                GoAhead();
            }
            else
            {
                bNext.IsEnabled = false;

                workerThread.Join();
                Draw(result);
                ShowInfo();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((workerThread != null) && workerThread.IsAlive)
                workerThread.Abort();
        }

        private void bEnd_Click(object sender, RoutedEventArgs e)
        {
            if (Algorithm is ListAlgorithm)
                (Algorithm as ListAlgorithm).IsPresentation = false;

            GoAhead();

            workerThread.Join();

            Elements = Algorithm.Result.Elements;

            DrawPreview();
            Draw(result);
            ShowInfo();
        }

        private void UpdateOpacity()
        {
            foreach (var bin in previewBins)
                bin.Border.Opacity = 1.0;
        }

        private void ShowInfo()
        {
            UpdateOpacity();

            laBinCount.Visibility = Visibility.Visible;
            laLowerBounds.Visibility = Visibility.Visible;
            laQualityEstimations.Visibility = Visibility.Visible;
            laErrorEstimations.Visibility = Visibility.Visible;

            //TODO: poprawić - powinno być tylko jeżeli IsPresentation było od początku
            if (!(Algorithm is ListAlgorithm) || !(Algorithm as ListAlgorithm).IsPresentation)
                laExecutionTime.Visibility = Visibility.Visible;

            tblMessage.Visibility = Visibility.Collapsed;
            bNext.Visibility = Visibility.Collapsed;
            bEnd.Visibility = Visibility.Collapsed;


            laBinCount.Content = "Liczba pudełek: " + result.Bins.Count;
            laExecutionTime.Content = "Czas obliczeń [ms]: " + stopWatch.ElapsedMilliseconds;

            int LB = Bounds.LowerBound(Elements, BinSize);
            int SLB = Bounds.StrongerLowerBound(Elements, BinSize, BinSize / 2 - 1);

            laLowerBounds.Content = String.Format("LB/SLB: {0}/{1}", LB, SLB);
            tblQualityEstimations.Text = String.Format("Oszac. jakości LB/SLB: {0:0.00}/{1:0.00}",
                (result.Bins.Count / (double)LB), (result.Bins.Count / (double)SLB));

            tblErrorEstimations.Text = String.Format("Oszac. błędu LB/SLB [%]: {0:0.00}/{1:0.00}",
                (100.0 * (result.Bins.Count - LB) / (double)LB),
                (100.0 * (result.Bins.Count - SLB) / (double)SLB));

            //int minBound = Math.Min(Bounds.LowerBound(Elements, BinSize), Bounds.StrongerLowerBound(Elements, BinSize, BinSize / 2 - 1));
            //laQualityEstimation.Content = "Oszacowanie jakości: " + (result.Bins.Count / (double)minBound).ToString("0.000");
            //laErrorEstimation.Content = "Oszacowanie błędu: " + (100.0 * (result.Bins.Count - minBound) / (double)minBound).ToString("0.000");


            //laLowerBound.Content = "LB: " + Bounds.LowerBound(Elements, BinSize);
            //laStrongerLowerBound.Content = "SLB: " + Bounds.StrongerLowerBound(Elements, BinSize, BinSize / 2 - 1);


            //<Label x:Name="laBinCount" Content="Liczba pudełek: " Visibility="Collapsed" />
            //            <Label x:Name="laExecutionTime" Content="Czas obliczeń: " Visibility="Collapsed" />
            //            <Label x:Name="laLowerBounds" Content="LB/SLB: " Visibility="Collapsed" />
            //            <Label x:Name="laQualityEstimations" Content="Oszac. jakości LB/SLB: " Visibility="Collapsed" />
            //            <Label x:Name="laErrorEstimations" Content="Oszac. błędu LB/SLB: " Visibility="Collapsed" />
        }
    }
}
