using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPacking.Library.Experiment
{
    public class ExperimentParamsFile
    {
        public int MinN { get; set; }
        public int MaxN { get; set; }
        public int Step { get; set; }

        public int BinSize { get; set; }
        public double MinVal { get; set; }
        public double MaxVal { get; set; }

        public int Repeat { get; set; }

        public double AASEpsilon { get; set; }

        public List<Algorithm> Algorithms { get; set; }
        public List<Distribution> Distributions { get; set; }
        public List<Sorting> Sortings { get; set; }

        public ExperimentParamsFile()
        {
            Algorithms = new List<Algorithm>();
            Distributions = new List<Distribution>();
            Sortings = new List<Sorting>();
        }
    }
}
