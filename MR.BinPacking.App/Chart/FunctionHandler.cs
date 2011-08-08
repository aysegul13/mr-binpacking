using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace MR.BinPacking.App.Chart
{
    public delegate double Func(double x);

    public class FunctionHandler
    {
        public string Name { get; set; }
        public bool Visible { get; set; }
        public Brush Color { get; set; }
        public Func Function { get; set; }

        public FunctionHandler()
        {
            Visible = false;
        }

        public static double FuncN(double x)
        {
            return x;
        }

        public static double FuncNN(double x)
        {
            return x * x;
        }

        public static double FuncLogN(double x)
        {
            return Math.Log(x, 2);
        }

        public static double FuncNLogN(double x)
        {
            return x * Math.Log(x, 2);
        }
    }
}
