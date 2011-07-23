using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace MR.BinPacking.App.Chart
{
    public class DataSerie
    {
        public string Name { get; set; }
        public bool Visible { get; set; }
        public Brush Color { get; set; }
        public List<Point2D> Points { get; set; }

        public DataSerie()
        {
            Visible = true;
            Points = new List<Point2D>();
        }
    }
}
