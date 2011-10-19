using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;

namespace MR.BinPacking.Library.Experiment
{
    public class Sample
    {
        public int ID { get; set; }

        public int N { get; set; }
        public Distribution Distribution { get; set; }
        public Sorting Sorting { get; set; }
        public Algorithm Algorithm { get; set; }
        public BaseAlgorithm Alg { get; set; }

        public double QualityEstimation { get; set; }
        public double ErrorEstimation { get; set; }
        public int Result { get; set; }
        public long ExecutionTime { get; set; }

        public double this[StatField index]
        {
            get
            {
                switch (index)
                {
                    case StatField.QualityEstimation:
                        return QualityEstimation;
                    case StatField.ErrorEstimation:
                        return ErrorEstimation;
                    case StatField.Result:
                        return Result;
                    case StatField.ExecutionTime:
                        return ExecutionTime;
                    default:
                        return N;
                }
            }
            private set { }
        }

        public void Add(Sample stats)
        {
            QualityEstimation += stats.QualityEstimation;
            ErrorEstimation += stats.ErrorEstimation;
            Result += stats.Result;
            ExecutionTime += stats.ExecutionTime;
        }
    }
}
