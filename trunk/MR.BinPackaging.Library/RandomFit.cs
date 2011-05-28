using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPackaging.Library.Base;
using System.Threading;

namespace MR.BinPackaging.Library
{
    public class RandomFit : IListAlgorithm
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

        public RandomFit()
        {
            Name = "Random Fit";
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

            Random random = new Random();

            foreach (var elem in result.Elements)
            {
                List<int> binsIndexes = new List<int>();
                for (int i = 0; i < result.Bins.Count; i++)
                {
                    if (result.Bins[i].FreeSpace() >= elem)
                        binsIndexes.Add(i);
                }

                int index;
                if (binsIndexes.Count == 0)
                {
                    result.Bins.Add(new Bin(result.BinSize));
                    index = result.Bins.Count - 1;
                }
                else
                {
                    index = binsIndexes[random.Next(binsIndexes.Count)];
                }

                result.Bins[index].Insert(elem);
            }

            return result;
        }
    }
}
