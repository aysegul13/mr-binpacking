using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPacking.Library.Experiment
{
    public class ExperimentResult
    {
        public ExperimentParams Params { get; set; }
        public List<Sample> Samples { get; set; }
    }
}
