using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPackaging.Library.Base;

namespace MR.BinPackaging.Library
{
    public class BestFit : IListAlgorithm
    {
        public string Name { get; private set; }

        public BestFit()
        {
            Name = "Best Fit";
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
