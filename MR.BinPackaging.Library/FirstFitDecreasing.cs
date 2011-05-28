using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPackaging.Library.Base;
using System.Threading;

namespace MR.BinPackaging.Library
{
    public class FirstFitDecreasing : IListAlgorithm
    {
        public string Name { get; private set; }

        //presentation properties
        public string Message { get; set; }
        public int SelectedElement { get; set; }
        public int SelectedBin { get; set; }
        public Instance ActualResult { get; set; }

        private volatile bool isWaiting = false;
        public bool IsWaiting
        {
            get
            {
                return isWaiting;
            }
            set
            {
                isWaiting = value;
            }
        }

        public FirstFitDecreasing()
        {
            Name = "First Fit Decreasing";
            IsWaiting = false;
        }

        public void Wait(int bin, int elem)
        {
            IsWaiting = true;
            SelectedBin = bin;
            SelectedElement = elem;

            Message = bin + "." + elem;

            while (IsWaiting)
                Thread.Sleep(100);
        }

        public Instance Execute(List<int> elements, int binSize)
        {
            List<int> elementsSorted = new List<int>(elements);
            elementsSorted.Sort((x, y) => y.CompareTo(x));

            return  new FirstFit().Execute(elementsSorted, binSize);
        }
    }
}
