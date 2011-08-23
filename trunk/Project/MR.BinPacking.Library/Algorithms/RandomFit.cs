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
            IsPresentation = true;

            PrevSelectedElement = -1;
            SelectedElement = -1;
        }

        public override Instance Execute(List<int> elements, int binSize)
        {
            Result = new Instance(binSize);
            Result.Elements = elements;

            List<int> elemIndexes = new List<int>();
            for (int i = 0; i < elements.Count; i++)
                elemIndexes.Add(i);

            Random random = new Random();

            while (elemIndexes.Count > 0)
            {
                int elemIndex = random.Next(elemIndexes.Count);

                #region UI
                if (IsPresentation)
                    Message += String.Format("Wylosowano element nr {0} ({1}).", elemIndexes[elemIndex] + 1, elements[elemIndexes[elemIndex]]) + Environment.NewLine + Environment.NewLine;
                #endregion

                List<int> binsIndexes = new List<int>();
                for (int i = 0; i < Result.Bins.Count; i++)
                {
                    if (Result.Bins[i].FreeSpace() >= elements[elemIndexes[elemIndex]])
                        binsIndexes.Add(i);
                }

                int index;
                if (binsIndexes.Count == 0)
                {
                    Result.Bins.Add(new Bin(Result.BinSize));
                    index = Result.Bins.Count - 1;

                    #region UI
                    if (IsPresentation)
                    {
                        Message += "Brak miejsca we wszystkich skrzynkach. Dodano nową skrzynkę." + Environment.NewLine + Environment.NewLine;
                        Wait(Result.Bins.Count - 1, elemIndexes[elemIndex]);
                    }
                    #endregion
                }
                else
                {
                    index = binsIndexes[random.Next(binsIndexes.Count)];

                    #region UI
                    if (IsPresentation)
                    {
                        Message += String.Format("Wylosowano skrzynkę nr {0}.", index + 1);
                        Wait(index, elemIndexes[elemIndex]);
                    }
                    #endregion
                }

                Result.Bins[index].Insert(elements[elemIndexes[elemIndex]]);

                #region UI
                if (IsPresentation)
                    Message = String.Format("Wstawiono element {0} ({1}) do skrzynki {2}.", elemIndexes[elemIndex] + 1, elements[elemIndexes[elemIndex]], index + 1) + Environment.NewLine + Environment.NewLine;
                #endregion

                elemIndexes.RemoveAt(elemIndex);
            }

            return Result;
        }
    }
}
