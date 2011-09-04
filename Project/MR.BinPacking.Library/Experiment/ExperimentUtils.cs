using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPacking.Library.Experiment
{
    public static class ExperimentUtils
    {
        public static String GetDistributionDisplayName(Distribution distribution)
        {
            switch (distribution)
            {
                case Distribution.Uniform:
                    return "jednostajny";
                case Distribution.Gauss:
                    return "normalny";
                case Distribution.Exponential:
                    return "wykładniczy";
                default:    //Distribution.None:
                    return "brak";
            }
        }

        public static String GetSortingDisplayName(Sorting sorting)
        {
            switch (sorting)
            {
                case Sorting.Ascending:
                    return "rosnąco";
                case Sorting.Descending:
                    return "malejąco";
                default:    //Sorting.None
                    return "brak";
            }
        }
    }
}
