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
using System.Collections.ObjectModel;
using MR.BinPacking.Library.Base;
using System.Windows.Media.Animation;

namespace MR.BinPacking.App.Controls
{
    /// <summary>
    /// Interaction logic for BinControl.xaml
    /// </summary>
    public partial class BinControl : UserControl
    {

        private ObservableCollection<ElementControl> dataItems;
        public ObservableCollection<ElementControl> DataItems
        {
            get { return dataItems; }
        }

        public bool AutoRefresh = true;

        public bool showScaled = true;
        public bool ShowScaled
        {
            get { return showScaled; }
            set
            {
                showScaled = value;
                UpdateLabels();
            }
        }

        public bool ShowFiller = true;

        public static DependencyProperty FreeSpaceProperty = DependencyProperty.Register(
            "FreeSpace", typeof(string), typeof(BinControl));
        public string FreeSpace
        {
            get
            {
                return (string)GetValue(FreeSpaceProperty);
            }
            set
            {
                SetValue(FreeSpaceProperty, value);
            }
        }


        public Bin bin;
        public Bin Bin
        {
            get
            {
                return bin;
            }
            set
            {
                bin = value;

                if (ShowScaled)
                    FreeSpace = (bin != null) ? ((double)bin.FreeSpace() / bin.Size).ToString("0.00") : "";
                else
                    FreeSpace = (bin != null) ? bin.FreeSpace().ToString() : "";

                dataItems.Clear();
                foreach (var binElem in value.Elements)
                {
                    ElementControl newElemControl = new ElementControl(binElem);
                    dataItems.Insert(0, newElemControl);
                }

                //UpdateSizes();
                UpdateLabels();
            }
        }


        public BinControl()
        {
            dataItems = new ObservableCollection<ElementControl>();
            InitializeComponent();

            Bin = new Bin();
        }


        public void StartAnimation()
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 1.0;
            doubleAnimation.To = 0.0;
            doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            doubleAnimation.AutoReverse = true;
            doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

            Border.BeginAnimation(Border.OpacityProperty, doubleAnimation);
        }

        public void StopAnimation()
        {
            Border.BeginAnimation(Border.OpacityProperty, null);
        }

        public void UpdateLabels()
        {
            if (ShowScaled)
                FreeSpace = (Bin != null) ? ((double)Bin.FreeSpace() / Bin.Size).ToString("0.00") : "";
            else
                FreeSpace = (Bin != null) ? Bin.FreeSpace().ToString() : "";

            foreach (var elem in dataItems)
            {
                if (ShowScaled)
                    elem.Message = ((double)elem.Size / Bin.Size).ToString("0.00");
                else
                    elem.Message = Bin.Size.ToString();
            }
        }

        public void UpdateSizes()
        {
            double height = Border.ActualHeight;
            if (ShowFiller && (Bin.FreeSpace() > 0))
            {
                bFiller.Height = height * Bin.FreeSpace() / Bin.Size;
                bFiller.Visibility = Visibility.Visible;
            }
            else
            {
                bFiller.Visibility = Visibility.Collapsed;
            }

            //if (bFiller.Visibility != Visibility.Collapsed)
            //    height -= bFiller.ActualHeight;

            foreach (var elem in dataItems)
            {
                //elem.Message = ((double)elem.Size / Bin.Size).ToString("0.00");
                elem.Height = height * elem.Size / Bin.Size;
            }

            dataItems.Add(new ElementControl(0));
            dataItems.Remove(dataItems.Last());
        }


        private double prevHeight = 0.0;
        private void binControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Math.Abs(ActualHeight - prevHeight) > 48)
            {
                prevHeight = ActualHeight;
                if (AutoRefresh)
                    UpdateSizes();
            }
        }

        private void Label_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Label senderLabel = (sender as Label);
            senderLabel.Foreground = Brushes.Red;
        }

        private void binControl_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSizes();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //StartAnimation();         
        }

        private void Border_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //StopAnimation();
        }
    }
}
