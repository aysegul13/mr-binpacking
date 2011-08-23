using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace MR.BinPacking.App.Chart
{
    public delegate double Func(double x, double maxX);

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

        public static double FuncN(double x, double maxX)
        {
            return x / maxX;
        }

        public static double FuncNN(double x, double maxX)
        {
            return (x * x) / (maxX * maxX);
        }

        public static double FuncLogN(double x, double maxX)
        {
            return Math.Log(x, 2) / Math.Log(maxX, 2);
        }

        public static double FuncNLogN(double x, double maxX)
        {
            return (x * Math.Log(x, 2)) / (maxX * Math.Log(maxX, 2));
        }

        public static double Func2PowN(double x, double maxX)
        {
            return Math.Pow(2, x - maxX);
        }

        public static double Const11f9(double x, double maxY)
        {
            return 11 / (9 * maxY);
        }

        public static double Const17f10(double x, double maxY)
        {
            return 1.7 / maxY;
        }

        public static double Const2(double x, double maxY)
        {
            return 2 / maxY;
        }
    }
}
