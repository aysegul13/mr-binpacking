using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using MR.BinPacking.Library.Utils;

namespace MR.BinPacking.Library.Experiment
{
    public class ExperimentParams
    {
        public int MinN { get; set; }
        public int MaxN { get; set; }
        public int Step { get; set; }
        public int Repeat { get; set; }
        public int BinSize { get; set; }
        public Distribution Dist { get; set; }
        public double MinVal { get; set; }
        public double MaxVal { get; set; }

        public List<ListAlgorithm> Algorithms { get; set; }
    }
}
