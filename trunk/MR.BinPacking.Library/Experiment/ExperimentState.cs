using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPacking.Library.Experiment
{
    public class ExperimentState
    {
        public int Repeat { get; set; }
        public int RepeatCount { get; set; }
        public int Step { get; set; }
        public int StepCount { get; set; }
        public int Algorithm { get; set; }
        public int AlgorithmCount { get; set; }

        public int N { get; set; }
        public string AlgorithmName { get; set; }
    }
}
