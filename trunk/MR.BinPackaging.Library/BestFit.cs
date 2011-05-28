using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPackaging.Library.Base;
using System.Threading;

namespace MR.BinPackaging.Library
{
    public class BestFit : IListAlgorithm
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

        public BestFit()
        {
            Name = "Best Fit";
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
            Instance result = new Instance(binSize);
            result.Elements = elements;

            foreach (var elem in result.Elements)
            {
                int minSpaceLeft = binSize;
                int minIndex = -1;

                for (int i = 0; i < result.Bins.Count; i++)
                {
                    if ((result.Bins[i].FreeSpace() >= elem) && (result.Bins[i].FreeSpace() - elem < minSpaceLeft))
                    {
                        minSpaceLeft = result.Bins[i].FreeSpace() - elem;
                        minIndex = i;
                    }
                }

                if (minIndex < 0)
                {
                    result.Bins.Add(new Bin(result.BinSize));
                    minIndex = result.Bins.Count - 1;
                }

                result.Bins[minIndex].Insert(elem);
            }

            return result;
        }
    }
}
