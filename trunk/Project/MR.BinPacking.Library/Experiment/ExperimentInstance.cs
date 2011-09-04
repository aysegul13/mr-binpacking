using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;

namespace MR.BinPacking.Library.Experiment
{
    public class ExperimentInstance : Instance
    {
        public Distribution Dist { get; set; }

        public ExperimentInstance(int binSize)
        {
            Elements = new List<int>();
            Bins = new List<Bin>();
            BinSize = binSize;

            Name = "";
        }

        public ExperimentInstance() : this(10) { }
    }
}
