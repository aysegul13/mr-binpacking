using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPackaging.Library.Base;
using System.Threading;

namespace MR.BinPackaging.Library
{
    public class NextFit : IListAlgorithm
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

        public NextFit()
        {
            Name = "Next Fit";
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
            //Instance result = new Instance(binSize);
            ActualResult = new Instance(binSize);
            ActualResult.Elements = elements;

            ActualResult.Bins.Add(new Bin(ActualResult.BinSize));
            int k = 0;
            //select bin
            //Wait(k, 0);
            int sum = 0;

            for (int i = 0; i < ActualResult.Elements.Count; i++)
            {
                //select bin, element
                Wait(k, i);

                int elem = ActualResult.Elements[i];
                if (sum + elem > ActualResult.BinSize)
                {
                    ActualResult.Bins.Add(new Bin(ActualResult.BinSize));
                    k++;

                    //select bin
                    Wait(k, i);
                    sum = 0;
                }

                ActualResult.Bins[k].Insert(elem);
                sum += elem;
            }

            return ActualResult;
        }
    }
}
