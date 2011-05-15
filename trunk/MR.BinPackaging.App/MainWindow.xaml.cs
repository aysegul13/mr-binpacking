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
using MR.BinPackaging.App.Controls;
using MR.BinPackaging.Library.Base;
using MR.BinPackaging.Library;
using MR.BinPackaging.App.Utils;
using System.Windows.Interop;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace MR.BinPackaging.App
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


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource source = HwndSource.FromVisual(this) as HwndSource;
            if (source != null)
            {
                source.AddHook(new HwndSourceHook(WinProc));
            }
        }

        public const Int32 WM_EXITSIZEMOVE = 0x0232;
        private IntPtr WinProc(IntPtr hwnd, Int32 msg, IntPtr wParam, IntPtr lParam, ref Boolean handled)
        {
            IntPtr result = IntPtr.Zero;
            switch (msg)
            {
                case WM_EXITSIZEMOVE:
                    {
                        //Refresh();
                        break;
                    }
            }

            return result;
        }

        private void bNextFit_Click(object sender, RoutedEventArgs e)
        {
            PreviewWindow prev = new PreviewWindow(new NextFit(), Elements, BinSize);
            prev.Show();
        }

        private void bFirstFit_Click(object sender, RoutedEventArgs e)
        {
            PreviewWindow prev = new PreviewWindow(new FirstFit(), Elements, BinSize);
            prev.Show();
        }

        private void bBestFit_Click(object sender, RoutedEventArgs e)
        {
            PreviewWindow prev = new PreviewWindow(new BestFit(), Elements, BinSize);
            prev.Show();
        }

        private void bFirstFitD_Click(object sender, RoutedEventArgs e)
        {
            PreviewWindow prev = new PreviewWindow(new FirstFitDecreasing(), Elements, BinSize);
            prev.Show();
        }

        private void bBestFitD_Click(object sender, RoutedEventArgs e)
        {
            PreviewWindow prev = new PreviewWindow(new BestFitDecreasing(), Elements, BinSize);
            prev.Show();
        }

        private void bUniformDist_Click(object sender, RoutedEventArgs e)
        {
            BinSize = Int32.Parse(ntbBinSize.Text);
            int elementsNumber = Int32.Parse(ntbElementsNumber.Text);
            int minValue = Int32.Parse(ntbMinValue.Text);
            int maxValue = Int32.Parse(ntbMaxValue.Text);

            Elements = Generator.GenerateData(elementsNumber, minValue, maxValue, Distribution.Uniform);

            RefreshPreview();
        }

        private void bGaussDist_Click(object sender, RoutedEventArgs e)
        {
            BinSize = Int32.Parse(ntbBinSize.Text);
            int elementsNumber = Int32.Parse(ntbElementsNumber.Text);
            int minValue = Int32.Parse(ntbMinValue.Text);
            int maxValue = Int32.Parse(ntbMaxValue.Text);

            Elements = Generator.GenerateData(elementsNumber, minValue, maxValue, Distribution.Gauss);

            RefreshPreview();
        }

        private void bExponentialDist_Click(object sender, RoutedEventArgs e)
        {
            BinSize = Int32.Parse(ntbBinSize.Text);
            int elementsNumber = Int32.Parse(ntbElementsNumber.Text);
            int minValue = Int32.Parse(ntbMinValue.Text);
            int maxValue = Int32.Parse(ntbMaxValue.Text);

            Elements = Generator.GenerateData(elementsNumber, minValue, maxValue, Distribution.Exponential);

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

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ExperimentParams prms = new ExperimentParams()
            {
                Algorithm = new NextFit(),
                BinSize = 100,
                Dist = Distribution.Uniform,
                MinN = 100,
                MaxN = 10000,
                Step = 100,
                MinVal = 0.0,
                MaxVal = 1.0,
                Repeat = 10
            };

            TestWindow test = new TestWindow(prms);
            test.Show();
        }

        private void bLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            if (openDialog.ShowDialog() == true)
            {
                Instance instance = Loader.LoadFromFile(openDialog.FileName);

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

        private void bRandomFit_Click(object sender, RoutedEventArgs e)
        {
            PreviewWindow prev = new PreviewWindow(new RandomFit(), Elements, BinSize);
            prev.Show();
        }
    }
}
