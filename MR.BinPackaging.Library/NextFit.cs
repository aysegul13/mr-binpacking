using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPackaging.Library.Base;

namespace MR.BinPackaging.Library
{
    public class NextFit : IListAlgorithm
    {
        public string Name { get; private set; }

        public NextFit()
        {
            Name = "Next Fit";
        }

        public Instance Execute(List<int> elements, int binSize)
        {
            Instance result = new Instance(binSize);
            result.Elements = elements;

            result.Bins.Add(new Bin(result.BinSize));
            int k = 0;
            int sum = 0;

            foreach (var elem in result.Elements)
            {
                if (sum + elem > result.BinSize)
                {
                    result.Bins.Add(new Bin(result.BinSize));
                    k++;
                    sum = 0;
                }

                result.Bins[k].Insert(elem);
                sum += elem;
            }

            return result;
        }
    }
}
