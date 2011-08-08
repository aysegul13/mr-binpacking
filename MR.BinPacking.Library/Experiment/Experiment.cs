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
            switch (sorting)
            {
                case Sorting.Ascending:
                    elements.Sort();
                    return elements;
                case Sorting.Descending:
                    List<int> elementsSorted = new List<int>(elements);
                    elementsSorted.Sort((x, y) => y.CompareTo(x));
                    return elementsSorted;
                default:
                    return elements;
            }
        }
    }
}
