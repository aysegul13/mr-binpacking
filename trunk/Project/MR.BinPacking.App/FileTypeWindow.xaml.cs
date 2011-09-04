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

namespace MR.BinPacking.App
{
    /// <summary>
    /// Interaction logic for FilteTypeWindow.xaml
    /// </summary>
    public partial class FileTypeWindow : Window
    {
        ExperimentInstance result = null;
        public ExperimentInstance Result
        {
            get { return result; }
        }

        string filename;

        private FileTypeWindow()
        {
            InitializeComponent();
        }

        public FileTypeWindow(string filename) : this()
        {
            InitTextBoxes();

            this.filename = filename;

            using (StreamReader sr = new StreamReader(filename))
                tbFile.Text = sr.ReadToEnd();
        }


        void InitTextBoxes()
        {
            tbFileTypeSimple.Text = "[wielkość_skrzynki]" + Environment.NewLine
                + "[liczba_elementów]" + Environment.NewLine
                + "element_1" + Environment.NewLine
                + "element_2" + Environment.NewLine
                + "..." + Environment.NewLine
                + "element_n" + Environment.NewLine;

            tbFileTypeMulti.Text = "liczba_instancji" + Environment.NewLine
                + "  nazwa_instancji_1" + Environment.NewLine
                + "  wielkość_skrzynki_1 liczba_elementów_1 [najlepsze_znane_rozw_1]" + Environment.NewLine
                + "    element_1_1" + Environment.NewLine
                + "    element_1_2" + Environment.NewLine
                + "    ..." + Environment.NewLine
                + "    element_1_m" + Environment.NewLine
                + "  nazwa_instancji_2" + Environment.NewLine
                + "  wielkość_skrzynki_2 liczba_elementów_2 [najlepsze_znane_rozw_2]" + Environment.NewLine
                + "    element_2_1" + Environment.NewLine
                + "    element_2_2" + Environment.NewLine
                + "    ..." + Environment.NewLine
                + "    element_2_m" + Environment.NewLine
                + "  ..." + Environment.NewLine
                + "  nazwa_instancji_n" + Environment.NewLine
                + "  wielkość_skrzynki_n liczba_elementów_n [najlepsze_znane_rozw_n]" + Environment.NewLine
                + "    element_n_1" + Environment.NewLine
                + "    element_n_2" + Environment.NewLine
                + "    ..." + Environment.NewLine
                + "    element_n_m" + Environment.NewLine;

            tbFiletypeMultiWithWeights.Text = "nazwa_instancji_1" + Environment.NewLine
                + "  liczba_wag_1" + Environment.NewLine
                + "  wielkość_skrzynki_1" + Environment.NewLine
                + "    waga_elem_1_1 liczba_elem_1_1" + Environment.NewLine
                + "    waga_elem_1_2 liczba_elem_1_2" + Environment.NewLine
                + "    ..." + Environment.NewLine
                + "    waga_elem_1_m liczba_elem_1_m" + Environment.NewLine
                + "nazwa_instancji_2" + Environment.NewLine
                + "  liczba_wag_2" + Environment.NewLine
                + "  wielkość_skrzynki_2" + Environment.NewLine
                + "    waga_elem_2_1 liczba_elem_2_1" + Environment.NewLine
                + "    waga_elem_2_2 liczba_elem_2_2" + Environment.NewLine
                + "    ..." + Environment.NewLine
                + "    waga_elem_2_m liczba_elem_2_m" + Environment.NewLine
                + "..." + Environment.NewLine
                + "nazwa_instancji_n" + Environment.NewLine
                + "  liczba_wag_n" + Environment.NewLine
                + "  wielkość_skrzynki_n" + Environment.NewLine
                + "    waga_elem_n_1 liczba_elem_n_1" + Environment.NewLine
                + "    waga_elem_n_2 liczba_elem_n_2" + Environment.NewLine
                + "    ..." + Environment.NewLine
                + "    waga_elem_n_m liczba_elem_n_m" + Environment.NewLine;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            InitTextBoxes();
        }

        private void bFileTypeSimple_Click(object sender, RoutedEventArgs e)
        {
            int elemCountIdx = -1;
            int binSizeIdx = -1;
            int binSize = Int32.Parse(ntbBinSize.Text);

            if (rbOnlyElemCount.IsChecked == true) { elemCountIdx = 0; }
            else if (rbOnlyBinSize.IsChecked == true) { binSizeIdx = 0; }
            else if (rbElemCountBinSize.IsChecked == true) { elemCountIdx = 0; binSizeIdx = 1; }
            else if (rbBinSizeElemCount.IsChecked == true) { elemCountIdx = 1; binSizeIdx = 0; }

            result = ESLoader.LoadFromFile1(filename, elemCountIdx, binSizeIdx, binSize);
            this.Close();
        }

        private void bFileTypeMulti_Click(object sender, RoutedEventArgs e)
        {
            List<ExperimentInstance> instances = ESLoader.LoadFromFile2(filename);
            tabFileTypes.IsEnabled = false;

            lbInstances.ItemsSource = instances;
            if (instances.Count > 0)
                lbInstances.SelectedIndex = 0;

            gInstances.Visibility = Visibility.Visible;
        }

        private void bFiletypeMultiWithWeights_Click(object sender, RoutedEventArgs e)
        {
            List<ExperimentInstance> instances = ESLoader.LoadFromFile3(filename);
            tabFileTypes.IsEnabled = false;

            lbInstances.ItemsSource = instances;
            if (instances.Count > 0)
                lbInstances.SelectedIndex = 0;

            gInstances.Visibility = Visibility.Visible;
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
            if ((lbInstances.SelectedItem != null) && (lbInstances.SelectedItem is ExperimentInstance))
            {
                result = lbInstances.SelectedItem as ExperimentInstance;
                this.Close();
            }
        }

        private void lbInstances_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bSelect.IsEnabled = (lbInstances.SelectedItem != null) && (lbInstances.SelectedItem is ExperimentInstance);
        }
    }
}
