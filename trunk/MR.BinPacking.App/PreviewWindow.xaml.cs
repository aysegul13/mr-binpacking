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
using MR.BinPackaging.Library.Base;
using MR.BinPackaging.App.Controls;
using System.Diagnostics;
using MR.BinPackaging.Library;
using System.Windows.Interop;
using System.Threading;

namespace MR.BinPackaging.App
{
    /// <summary>
    /// Interaction logic for PreviewWindow.xaml
    /// </summary>
    public partial class PreviewWindow : Window
    {
        private ListAlgorithm Algorithm;
        private List<int> Elements;
        private int BinSize = 10;

        private List<BinControl> previewBins = new List<BinControl>();
        private List<BinControl> algorithmBins = new List<BinControl>();

        public PreviewWindow(ListAlgorithm algorithm, List<int> elements, int binSize)
        {
            InitializeComponent();

            Algorithm = algorithm;
            Elements = elements;
            BinSize = binSize;

            this.Title = algorithm.Name;
        }


        public const Int32 WM_EXITSIZEMOVE = 0x0232;
        private IntPtr WinProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, ref Boolean handled)
        {
            IntPtr result = IntPtr.Zero;
            switch (msg)
            {
                case WM_EXITSIZEMOVE:
                    {
                        Refresh();
                        break;
                    }
            }

            return result;
        }

        private void Refresh()
        {
            foreach (var child in spResult.Children)
            {
                if (child is BinControl)
                {
                    BinControl binControl = child as BinControl;
                    if (binControl.AutoRefresh)
                        binControl.UpdateSizes();
                }
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = HwndSource.FromVisual(this) as HwndSource;
            if (source != null)
            {
                source.AddHook(new HwndSourceHook(WinProc));
            }
        }


        public void DoWork()
        {
            result = Algorithm.Execute(Elements, BinSize);
        }

        public void GoAhead()
        {
            Algorithm.IsWaiting = false;
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
                newBin.AutoRefresh = false;
                newBin.Bin = bin;
                newBin.ShowFiller = false;

                newBin.LayoutTransform = new ScaleTransform(0.8, 0.8);
                newBin.Border.BorderThickness = new Thickness(0);
                newBin.Border.Background = Brushes.Transparent;
                newBin.laFreeSpace.Visibility = Visibility.Collapsed;

                foreach (var elemControl in newBin.DataItems)
                    elemControl.Border.BorderThickness = new Thickness(3);

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

                newBin.UpdateSizes();

                //int sum = 0;
                //foreach (var elem in bin.Elements)
                //{
                //    double totalSum = (totalHeight * sum) / bin.Size;
                //    double rectHeight = (totalHeight * elem) / bin.Size;
                //    double rectTop = 36 + totalHeight - totalSum - rectHeight;

                //    Rectangle rect = new Rectangle();
                //    rect.Fill = Brushes.Beige;
                //    rect.Width = 64 - 8;
                //    rect.Height = rectHeight;

                //    Canvas.SetTop(rect, rectTop);
                //    Canvas.SetLeft(rect, 64 * binCnt + 4);

                //    Line line = new Line();
                //    line.X1 = 64 * binCnt + 4;
                //    line.X2 = line.X1 + 64 - 8;
                //    line.Y1 = line.Y2 = rectTop;
                //    line.StrokeThickness = 3;
                //    line.Stroke = Brushes.Black;

                //    cnResult.Children.Add(rect);
                //    cnResult.Children.Add(line);

                //    sum += elem;
                //}

                //binCnt++;
            }


            if (Algorithm.SelectedElement > 0)
                previewBins[Algorithm.SelectedElement - 1].StopAnimation();

            if (Algorithm.SelectedElement >= 0)
                previewBins[Algorithm.SelectedElement].StartAnimation();

            if (Algorithm.SelectedBin >= 0)
                algorithmBins[Algorithm.SelectedBin].StartAnimation();

            if (result != null)
            {
                previewBins[Algorithm.SelectedElement].StopAnimation();
                algorithmBins[Algorithm.SelectedBin].StopAnimation();
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
            DrawPreview();
            Execute();
        }

        private bool first = true;
        private void bNext_Click(object sender, RoutedEventArgs e)
        {
            if (result == null)
            {
                tblMessage.Text = Algorithm.Message;

                Elements = Algorithm.ActualResult.Elements;

                Instance inst = new Instance(Algorithm.ActualResult.BinSize);
                foreach (var bin in Algorithm.ActualResult.Bins)
                {
                    Bin newBin = new Bin(Algorithm.ActualResult.BinSize);

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
                tblMessage.Text = Algorithm.Message;

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
            Algorithm.IsPresentation = false;
            GoAhead();

            workerThread.Join();
            Draw(result);
            ShowInfo();
        }

        private void ShowInfo()
        {
            laBinCount.Visibility = Visibility.Visible;
            laLowerBounds.Visibility = Visibility.Visible;
            laQualityEstimations.Visibility = Visibility.Visible;
            laErrorEstimations.Visibility = Visibility.Visible;

            //TODO: poprawić - powinno być tylko jeżeli IsPresentation było od początku
            if (!Algorithm.IsPresentation)
                laExecutionTime.Visibility = Visibility.Visible;

            tblMessage.Visibility = Visibility.Hidden;
            bNext.Visibility = Visibility.Hidden;
            bEnd.Visibility = Visibility.Hidden;
        }
    }
}
