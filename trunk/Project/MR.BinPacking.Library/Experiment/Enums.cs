using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPacking.Library.Experiment
{
    public enum Distribution { Uniform, Gauss, Exponential, None };
    public enum Sorting { None, Ascending, Descending };
    public enum Algorithm
    {
        NextFit,
        FirstFit,
        BestFit,
        FirstFitDecreasing,
        BestFitDecreasing,
        RandomFit,
        AsymptoticApproximationScheme,
        Reduction,
        Exact,
        PBI
    };
}
