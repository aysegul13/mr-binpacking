using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPackaging.Library.Base;

namespace MR.BinPackaging.Library
{
    public class BestFitDecreasing : IListAlgorithm
    {
        public string Name { get; private set; }

        public BestFitDecreasing()
        {
            Name = "Best Fit Decreasing";
        }

        public Instance Execute(List<int> elements, int binSize)
        {
            List<int> elementsSorted = new List<int>(elements);
            elementsSorted.Sort((x, y) => y.CompareTo(x));

            return  new BestFit().Execute(elementsSorted, binSize);
        }
    }
}
