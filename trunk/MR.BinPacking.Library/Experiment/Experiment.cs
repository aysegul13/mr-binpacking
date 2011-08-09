using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using System.Diagnostics;
using MR.BinPacking.Library.Utils;

namespace MR.BinPacking.Library.Experiment
{
    public static class Experiment
    {
        public static List<int> GetElementsWithSorting(List<int> elements, Sorting sorting)
        {
            List<int> result = new List<int>(elements);

            switch (sorting)
            {
                case Sorting.Ascending:
                    result.Sort();
                    return result;
                case Sorting.Descending:
                    result.Sort((x, y) => y.CompareTo(x));
                    return result;
                default:
                    return result;
            }
        }
    }
}
