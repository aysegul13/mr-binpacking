using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MR.BinPackaging.App.Controls
{
    public class ElemTest : DependencyObject
    {
        public static DependencyProperty MessageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(ElemTest));
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

        public static DependencyProperty HeightProperty = DependencyProperty.Register(
            "Height", typeof(double), typeof(ElemTest));
        public double Height
        {
            get
            {
                return (double)GetValue(HeightProperty);
            }
            set
            {
                SetValue(HeightProperty, value);
            }

        }

        public ElemTest(string msg, double height)
        {
            Message = msg;
            Height = height;
        }
    }
}
