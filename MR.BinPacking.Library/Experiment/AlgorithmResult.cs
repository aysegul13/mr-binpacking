using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPacking.Library.Experiment
{
    public class AlgorithmResult
    {
        public List<Sample> Result { get; set; }

        public AlgorithmResult()
        {
            Result = new List<Sample>();
        }

        public Sample this[int index]
        {
            get { return Result[index]; }
            set { Result[index] = value; }
        }
    }
}
