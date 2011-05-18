using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPackaging.Library.Base;

namespace MR.BinPackaging.Library
{
    public class FirstFit : IListAlgorithm
    {
        public string Name { get; private set; }

        //presentation properties
        public string Message { get; set; }
        public int SelectedElement { get; set; }
        public int SelectedBin { get; set; }
        public bool IsWaiting { get; set; }
        public Instance ActualResult { get; set; }

        public FirstFit()
        {
            Name = "First Fit";
            IsWaiting = false;
        }

        public Instance Execute(List<int> elements, int binSize)
        {
            Instance result = new Instance(binSize);
            result.Elements = elements;

            foreach (var elem in result.Elements)
            {
                bool fit = false;
                foreach (var bin in result.Bins)
                {
                    if (bin.FreeSpace() >= elem)
                    {
                        fit = true;
                        bin.Insert(elem);
                        break;
                    }
                }

                if (!fit)
                {
                    result.Bins.Add(new Bin(result.BinSize));
                    result.Bins.Last().Insert(elem);
                }
            }

            return result;
        }
    }
}
