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

namespace MR.BinPacking.App.Controls
{
    /// <summary>
    /// Interaction logic for ElementControl.xaml
    /// </summary>
    public partial class ElementControl : UserControl
    {
        public static Brush fillBrush = Brushes.LightPink;
        public static Brush borderBrush = Brushes.CornflowerBlue;
        public static Brush fontForeground = Brushes.OrangeRed;

        public ElementControl() : this(0) { }

        public ElementControl(int size)
        {
            InitializeComponent();

            Color = fillBrush;
            Border.BorderBrush = ElementControl.borderBrush;
            laSize.Foreground = ElementControl.fontForeground;

            Size = size;
            Message = Size.ToString();
        }


        public int Size { get; set; }


        public static DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(ElementControl));
        public string Message
        {
            get
            {
                return (string)GetValue(MessageProperty);
            }
            set
            {
                SetValue(MessageProperty, value);
            }
        }

        public static DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color", typeof(Brush), typeof(ElementControl));
        public Brush Color
        {
            get
            {
                return (Brush)GetValue(ColorProperty);
            }
            set
            {
                SetValue(ColorProperty, value);
            }
        }
    }
}
