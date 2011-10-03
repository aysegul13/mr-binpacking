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
using System.IO;
using MR.BinPacking.Library.Base;
using MR.BinPacking.App.Utils;
using MR.BinPacking.Library.Experiment;
using MR.BinPacking.App.Properties;
using System.Reflection;
using System.Threading;

namespace MR.BinPacking.App
{
    /// <summary>
    /// Interaction logic for FilteTypeWindow.xaml
    /// </summary>
    public partial class FileTypeWindow : Window
    {
        ExpInstance result = null;
        public ExpInstance Result
        {
            get { return result; }
        }

        string filename;

        private FileTypeWindow()
        {
            InitializeComponent();
        }

        public FileTypeWindow(string filename)
            : this()
        {
            InitTextBoxes();

            try
            {
                this.filename = filename;
                using (StreamReader sr = new StreamReader(filename))
                    tbFile.Text = sr.ReadToEnd();
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }


        void InitTextBoxes()
        {
            tbFileTypeSimple.Text = "[rozmiar_pudełek]" + Environment.NewLine
                + "[liczba_elementów]" + Environment.NewLine
                + "element_1" + Environment.NewLine
                + "element_2" + Environment.NewLine
                + "..." + Environment.NewLine
                + "element_n" + Environment.NewLine;

            tbFileTypeMulti.Text = "liczba_instancji" + Environment.NewLine
                + "  nazwa_instancji_1" + Environment.NewLine
                + "  rozmiar_pudełek_1 liczba_elementów_1 [najlepsze_znane_rozw_1]" + Environment.NewLine
                + "    element_1_1" + Environment.NewLine
                + "    element_1_2" + Environment.NewLine
                + "    ..." + Environment.NewLine
                + "    element_1_m" + Environment.NewLine
                + "  nazwa_instancji_2" + Environment.NewLine
                + "  rozmiar_pudełek_2 liczba_elementów_2 [najlepsze_znane_rozw_2]" + Environment.NewLine
                + "    element_2_1" + Environment.NewLine
                + "    element_2_2" + Environment.NewLine
                + "    ..." + Environment.NewLine
                + "    element_2_m" + Environment.NewLine
                + "  ..." + Environment.NewLine
                + "  nazwa_instancji_n" + Environment.NewLine
                + "  rozmiar_pudełek_n liczba_elementów_n [najlepsze_znane_rozw_n]" + Environment.NewLine
                + "    element_n_1" + Environment.NewLine
                + "    element_n_2" + Environment.NewLine
                + "    ..." + Environment.NewLine
                + "    element_n_m" + Environment.NewLine;

            tbFiletypeMultiWithWeights.Text = "nazwa_instancji_1" + Environment.NewLine
                + "  liczba_rozmiarów_elementów_1" + Environment.NewLine
                + "  rozmiar_pudełek_1" + Environment.NewLine
                + "    rozmiar_elem_1_1 liczba_elem_1_1" + Environment.NewLine
                + "    rozmiar_elem_1_2 liczba_elem_1_2" + Environment.NewLine
                + "    ..." + Environment.NewLine
                + "    rozmiar_elem_1_m liczba_elem_1_m" + Environment.NewLine
                + "nazwa_instancji_2" + Environment.NewLine
                + "  liczba_rozmiarów_elementów_2" + Environment.NewLine
                + "  rozmiar_pudełek_2" + Environment.NewLine
                + "    rozmiar_elem_2_1 liczba_elem_2_1" + Environment.NewLine
                + "    rozmiar_elem_2_2 liczba_elem_2_2" + Environment.NewLine
                + "    ..." + Environment.NewLine
                + "    rozmiar_elem_2_m liczba_elem_2_m" + Environment.NewLine
                + "..." + Environment.NewLine
                + "nazwa_instancji_n" + Environment.NewLine
                + "  liczba_rozmiarów_elementów_n" + Environment.NewLine
                + "  rozmiar_pudełek_n" + Environment.NewLine
                + "    rozmiar_elem_n_1 liczba_elem_n_1" + Environment.NewLine
                + "    rozmiar_elem_n_2 liczba_elem_n_2" + Environment.NewLine
                + "    ..." + Environment.NewLine
                + "    rozmiar_elem_n_m liczba_elem_n_m" + Environment.NewLine;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitTextBoxes();

            switch (Settings.Default.LastFileType)
            {
                case FileType.Simple:
                    tabFileTypes.SelectedIndex = 0;
                    break;
                case FileType.Multi:
                    tabFileTypes.SelectedIndex = 1;
                    break;
                case FileType.MultiWithWeights:
                    tabFileTypes.SelectedIndex = 2;
                    break;
            }

            try
            {
                rbHeaderEmpty.IsChecked = (Settings.Default.LastFileHeaderType == 0);
                rbOnlyBinSize.IsChecked = (Settings.Default.LastFileHeaderType == 1);
                rbOnlyElemCount.IsChecked = (Settings.Default.LastFileHeaderType == 2);
                rbBinSizeElemCount.IsChecked = (Settings.Default.LastFileHeaderType == 3);
                rbElemCountBinSize.IsChecked = (Settings.Default.LastFileHeaderType == 4);
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void bFileTypeSimple_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int elemCountIdx = -1;
                int binSizeIdx = -1;
                int binSize = Int32.Parse(ntbBinSize.Text);

                if (rbOnlyElemCount.IsChecked == true) { elemCountIdx = 0; }
                else if (rbOnlyBinSize.IsChecked == true) { binSizeIdx = 0; }
                else if (rbElemCountBinSize.IsChecked == true) { elemCountIdx = 0; binSizeIdx = 1; }
                else if (rbBinSizeElemCount.IsChecked == true) { elemCountIdx = 1; binSizeIdx = 0; }

                result = Loader.LoadInstance1(filename, elemCountIdx, binSizeIdx, binSize);
                this.Close();
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void bFileTypeMulti_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<ExpInstance> instances = Loader.LoadInstance2(filename);
                tabFileTypes.IsEnabled = false;

                lbInstances.ItemsSource = instances;
                if (instances.Count > 0)
                    lbInstances.SelectedIndex = 0;

                gInstances.Visibility = Visibility.Visible;
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void bFiletypeMultiWithWeights_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<ExpInstance> instances = Loader.LoadInstance3(filename);
                tabFileTypes.IsEnabled = false;

                lbInstances.ItemsSource = instances;
                if (instances.Count > 0)
                    lbInstances.SelectedIndex = 0;

                gInstances.Visibility = Visibility.Visible;
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void rbHeaderEmpty_Checked(object sender, RoutedEventArgs e)
        {
            if (ntbBinSize != null)
                ntbBinSize.IsEnabled = true;
        }

        private void rbOnlyBinSize_Checked(object sender, RoutedEventArgs e)
        {
            ntbBinSize.IsEnabled = false;
        }

        private void rbOnlyElemCount_Checked(object sender, RoutedEventArgs e)
        {
            ntbBinSize.IsEnabled = true;
        }

        private void rbBinSizeElemCount_Checked(object sender, RoutedEventArgs e)
        {
            ntbBinSize.IsEnabled = false;
        }

        private void rbElemCountBinSize_Checked(object sender, RoutedEventArgs e)
        {
            ntbBinSize.IsEnabled = false;
        }

        private void bSelect_Click(object sender, RoutedEventArgs e)
        {
            if ((lbInstances.SelectedItem != null) && (lbInstances.SelectedItem is ExpInstance))
            {
                result = lbInstances.SelectedItem as ExpInstance;
                this.Close();
            }
        }

        private void lbInstances_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bSelect.IsEnabled = (lbInstances.SelectedItem != null) && (lbInstances.SelectedItem is ExpInstance);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tabFileTypes.SelectedIndex == 2)
                Settings.Default.LastFileType = FileType.MultiWithWeights;
            else if (tabFileTypes.SelectedIndex == 1)
                Settings.Default.LastFileType = FileType.Multi;
            else
                Settings.Default.LastFileType = FileType.Simple;

            if (rbHeaderEmpty.IsChecked == true)
                Settings.Default.LastFileHeaderType = 0;
            else if (rbOnlyBinSize.IsChecked == true)
                Settings.Default.LastFileHeaderType = 1;
            else if (rbOnlyElemCount.IsChecked == true)
                Settings.Default.LastFileHeaderType = 2;
            else if (rbBinSizeElemCount.IsChecked == true)
                Settings.Default.LastFileHeaderType = 3;
            else if (rbElemCountBinSize.IsChecked == true)
                Settings.Default.LastFileHeaderType = 4;
        }
    }
}
