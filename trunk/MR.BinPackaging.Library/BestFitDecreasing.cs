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

        //presentation properties
        public string Message { get; set; }
        public int SelectedElement { get; set; }
        public int SelectedBin { get; set; }
        public bool IsWaiting { get; set; }
        public Instance ActualResult { get; set; }

        public BestFitDecreasing()
        {
            Name = "Best Fit Decreasing";
            IsWaiting = false;
        }

        public Instance Execute(List<int> elements, int binSize)
        {
            List<int> elementsSorted = new List<int>(elements);
            elementsSorted.Sort((x, y) => y.CompareTo(x));

            return  new BestFit().Execute(elementsSorted, binSize);
        }
    }
}
