using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using MR.BinPacking.Library.Utils;

namespace MR.BinPacking.Library.Experiment
{
    public enum Distribution { Uniform, Gauss, Exponential };
    public enum Sorting { None, Ascending, Descending };
    public enum Algorithm {
        NextFit,
        FirstFit,
        BestFit,
        FirstFitDecreasing,
        BestFitDecreasing,
        RandomFit,
        AsymptoticApproximationScheme,
        BruteForce
    };

    public class ExperimentParamsFile
    {
        public int MinN { get; set; }
        public int MaxN { get; set; }
        public int Step { get; set; }

        public int BinSize { get; set; }
        public double MinVal { get; set; }
        public double MaxVal { get; set; }

        public int Repeat { get; set; }
        public Sorting Sorting { get; set; }

        public List<Algorithm> Algs { get; set; }
        public List<Distribution> Distributions { get; set; }
    }

    public class ExperimentParams : ExperimentParamsFile
    {
        public List<ListAlgorithm> Algorithms { get; set; }
    }
}
