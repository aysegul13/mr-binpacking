using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPackaging.Library.Base;
using System.Threading;

namespace MR.BinPackaging.Library
{
    public class FirstFit : IListAlgorithm
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

        public FirstFit()
        {
            Name = "First Fit";
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
                //select bin, element
                //Wait(0, i);

                int elem = ActualResult.Elements[i];

                bool fit = false;
                for (int k = 0; k < ActualResult.Bins.Count; k++)
                {
                    //select bin
                    Wait(k, i);

                    Bin bin = ActualResult.Bins[k];
                    if (bin.FreeSpace() >= elem)
                    {
                        fit = true;
                        bin.Insert(elem);
                        break;
                    }
                }

                if (!fit)
                {
                    ActualResult.Bins.Add(new Bin(ActualResult.BinSize));

                    //select bin
                    Wait(ActualResult.Bins.Count - 1, i);

                    ActualResult.Bins.Last().Insert(elem);
                }
            }

            return ActualResult;
        }
    }
}
