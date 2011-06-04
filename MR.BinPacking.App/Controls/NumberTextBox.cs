using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace MR.BinPacking.App.Controls
{
    public class NumberTextBox : TextBox
    {
        private int minValue = 0;
        public int MinValue
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

        private int maxValue = 100;
        public int MaxValue
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
                if (!Char.IsDigit(str[i]))
                    return false;
            }

            return true;
        }

        private void FixValue()
        {
            if ((Text.Length == 0) || (Int32.Parse(Text) < MinValue))
                Text = MinValue.ToString();
            else if (Int32.Parse(Text) > MaxValue)
                Text = MaxValue.ToString();
        }

        protected override void OnLostFocus(System.Windows.RoutedEventArgs e)
        {
            FixValue();

            base.OnLostFocus(e);
        }
    }
}
