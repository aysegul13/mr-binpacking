using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Experiment;

namespace MR.BinPacking.Library.Base
{
    public class Instance
    {
        public List<int> Elements { get; set; }
        public List<Bin> Bins { get; set; }

        public int BinSize { get; set; }

        public string Name { get; set; }

        public Instance(int binSize)
        {
            Elements = new List<int>();
            Bins = new List<Bin>();
            BinSize = binSize;

            Name = "";
        }

        public Instance() : this(10) { }
    }

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
