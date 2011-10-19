using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace MR.BinPacking.App.Controls
{
    public class NumberTextBox : TextBox
    {
        private bool intsOnly = true;
        public bool IntsOnly
        {
            get { return intsOnly; }
            set { intsOnly = value; }
        }

        private double minValue = 0;
        public double MinValue
        {
            get
            {
                return minValue;
            }
            set
            {
                if (value < 0)
                    minValue = 0;
                else
                    minValue = value;

                FixValue();
            }
        }

        private double maxValue = 100;
        public double MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                if (value < 0)
                    maxValue = 0;
                else
                    maxValue = value;

                FixValue();
            }
        }

        public NumberTextBox()
        {
            Text = MinValue.ToString();
        }

        protected override void OnPreviewTextInput(System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !InputValid(e.Text);

            base.OnPreviewTextInput(e);
        }

        private bool InputValid(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];

                if (!Char.IsDigit(c) && (IntsOnly || ((c != '.') && (c != ','))))
                    return false;
            }

            return true;
        }

        private void FixValue()
        {
            if (IntsOnly)
            {
                int val;
                if (!Int32.TryParse(Text, out val) || (val < MinValue))
                    Text = MinValue.ToString();
                else if (val > MaxValue)
                    Text = MaxValue.ToString();
            }
            else
            {
                double val;
                if (!Double.TryParse(Text, out val) || (val < MinValue))
                    Text = MinValue.ToString();
                else if (val > MaxValue)
                    Text = MaxValue.ToString();
            }
        }

        protected override void OnLostFocus(System.Windows.RoutedEventArgs e)
        {
            FixValue();

            base.OnLostFocus(e);
        }
    }
}
