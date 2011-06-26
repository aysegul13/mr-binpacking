﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPacking.Library.Experiment
{
    public class Statistics
    {
        public int N { get; set; }
        public int LowerBound { get; set; }
        public int StrongerLowerBound { get; set; }
        public int Result { get; set; }
        public long ExecutionTime { get; set; }

        public long this[StatField index]
        {
            get
            {
                switch (index)
                {
                    case StatField.LowerBound:
                        return LowerBound;
                    case StatField.StrongerLowerBound:
                        return StrongerLowerBound;
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

        public void Add(Statistics stats)
        {
            LowerBound += stats.LowerBound;
            StrongerLowerBound += stats.StrongerLowerBound;
            Result += stats.Result;
            ExecutionTime += stats.ExecutionTime;
        }
    }
}