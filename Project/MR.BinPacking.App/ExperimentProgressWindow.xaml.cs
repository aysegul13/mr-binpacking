using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using MR.BinPacking.Library.Experiment;
using System.ComponentModel;
using MR.BinPacking.Library.Utils;
using System.Diagnostics;
using MR.BinPacking.Library.Base;
using MR.BinPacking.Library;
using System.Threading;
using System.Reflection;

namespace MR.BinPacking.App
{
    /// <summary>
    /// Interaction logic for ExperimentProgressWindow.xaml
    /// </summary>
    public partial class ExperimentProgressWindow : Window
    {
        ExpParams experimentParams = null;
        ExpInstance experimentInstance = null;
        Thread workerThread = null;

        public ExperimentProgressWindow(ExpParams prms, ExpInstance instance)
        {
            InitializeComponent();

            experimentParams = prms;
            experimentInstance = instance;
        }

        void ReportProgress(ExpState state)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        laRepeat.Content = String.Format("Powtórzenie: {0}", state.Repeat + 1);
                        laStep.Content = String.Format("Liczba elementów: {0}", state.N);

                        laDistribution.Content = String.Format("Rozkład: {0}", ExpUtils.GetDistributionDisplayName(state.Distribution));
                        laSorting.Content = String.Format("Sortowanie: {0}", ExpUtils.GetSortingDisplayName(state.Sorting));

                        laAlgorithm.Content = String.Format("Algorytm: {0}", state.AlgorithmName);

                        laPart.Content = String.Format("Krok: {0} z {1}", state.ActualSample, state.Samples);
                        pbPart.Maximum = 1.0;
                        pbPart.Value = (double)(state.ActualSample) / state.Samples;
                    }));
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        void Complete(ExpResult result)
        {
            try
            {
                this.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        guiChart.DataSource = result;

                        spExperimentProgress.Visibility = pbPart.Visibility = bRun.Visibility = bCancel.Visibility = Visibility.Collapsed;
                        guiChart.Visibility = Visibility.Visible;

                        this.Width = 600;
                        this.Height = 450;

                        guiChart.GetParamsAndRefresh();
                    }));
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void bCancel_Click(object sender, RoutedEventArgs e)
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

            bCancel.IsEnabled = false;
            bRun.IsEnabled = false;
        }

        private void bRun_Click(object sender, RoutedEventArgs e)
        {
            bCancel.IsEnabled = true;
            bRun.IsEnabled = false;

            try
            {
                Worker worker = new Worker(experimentParams, experimentInstance)
                {
                    ReportProgress = this.ReportProgress,
                    Complete = this.Complete,
                    ShowError = MainWindow.ShowError
                };

                if (experimentInstance == null)
                    workerThread = new Thread(worker.DoWorkGenerator);
                else
                    workerThread = new Thread(worker.DoWorkFile);

                workerThread.Start();

                //TODO: this may cause errors!!!
                while (!workerThread.IsAlive) ;
            }
            catch (ThreadAbortException) { }
            catch (Exception exc)
            {
                MainWindow.ShowError(exc, MethodBase.GetCurrentMethod().Name);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
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
    }
}
