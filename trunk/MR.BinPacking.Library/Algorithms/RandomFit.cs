using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using System.Threading;

namespace MR.BinPacking.Library.Algorithms
{
    public class RandomFit : ListAlgorithm
    {
        public RandomFit()
        {
            Name = "Random Fit";
            IsWaiting = false;
        }

        public override Instance Execute(List<int> elements, int binSize)
        {
            //Instance result = new Instance(binSize);
            //result.Elements = elements;
            ActualResult = new Instance(binSize);
            ActualResult.Elements = elements;

            List<int> elemIndexes = new List<int>();
            for (int i = 0; i < elements.Count; i++)
                elemIndexes.Add(i);

            Random random = new Random();

            while (elemIndexes.Count > 0)
            {
                int elemIndex = random.Next(elemIndexes.Count);

                List<int> binsIndexes = new List<int>();
                for (int i = 0; i < ActualResult.Bins.Count; i++)
                {
                    if (ActualResult.Bins[i].FreeSpace() >= elements[elemIndexes[elemIndex]])
                        binsIndexes.Add(i);
                }

                int index;
                if (binsIndexes.Count == 0)
                {
                    ActualResult.Bins.Add(new Bin(ActualResult.BinSize));
                    index = ActualResult.Bins.Count - 1;
                }
                else
                {
                    index = binsIndexes[random.Next(binsIndexes.Count)];
                }

                ActualResult.Bins[index].Insert(elements[elemIndexes[elemIndex]]);
                elemIndexes.RemoveAt(elemIndex);
            }

            return ActualResult;
        }
    }
}
