using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPacking.Library.Experiment
{
    public class AlgorithmResult
    {
        public List<Statistics> Result { get; set; }

        public AlgorithmResult()
        {
            Result = new List<Statistics>();
        }

        public Statistics this[int index]
        {
            get { return Result[index]; }
            set { Result[index] = value; }
        }
    }
}
