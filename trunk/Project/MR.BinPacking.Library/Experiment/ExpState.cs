using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPacking.Library.Experiment
{
    public class ExpState
    {
        public int Repeat { get; set; }
        public int N { get; set; }
        public Distribution Distribution { get; set; }
        public Sorting Sorting { get; set; }
        public string AlgorithmName { get; set; }

        public int ActualSample { get; set; }
        public int Samples { get; set; }
    }
}
