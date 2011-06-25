using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPacking.Library.Experiment
{
    public class Point2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point2D() : this(0.0, 0.0) { }
        public Point2D(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
