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
            ActualResult = new Instance(binSize);
            ActualResult.Elements = elements;

            for (int i = 0; i < ActualResult.Elements.Count; i++)
            {
                int elem = ActualResult.Elements[i];
                int minSpaceLeft = binSize;
                int minIndex = -1;

                for (int k = 0; k < ActualResult.Bins.Count; k++)
                {
                    //select bin
                    Wait(k, i);

                    if ((ActualResult.Bins[k].FreeSpace() >= elem) && (ActualResult.Bins[k].FreeSpace() - elem < minSpaceLeft))
                    {
                        minSpaceLeft = ActualResult.Bins[k].FreeSpace() - elem;
                        minIndex = k;
                    }
                }

                if (minIndex < 0)
                {
                    ActualResult.Bins.Add(new Bin(ActualResult.BinSize));
                    minIndex = ActualResult.Bins.Count - 1;

                    //select bin
                    Wait(minIndex, i);
                }

                ActualResult.Bins[minIndex].Insert(elem);
            }

            return ActualResult;
        }
    }
}
