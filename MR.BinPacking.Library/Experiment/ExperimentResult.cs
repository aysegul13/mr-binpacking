using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPacking.Library.Experiment
{
    public class ExperimentResult
    {
        public ExperimentParams Params { get; set; }
        public List<AlgorithmResult> DataSeries { get; set; }

        public ExperimentResult()
        {
            DataSeries = new List<AlgorithmResult>();
        }

        public AlgorithmResult this[int index]
        {
            get { return DataSeries[index]; }
            set { DataSeries[index] = value; }
        }
    }
}
