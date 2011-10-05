﻿using System;
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
                string elemStr = ShowScaled ? ((double)elements[elemIndexes[elemIndex]] / binSize).ToString("0.####") : elements[elemIndexes[elemIndex]].ToString();

                #region UI
                if (IsPresentation)
                    Message += String.Format("Wylosowano element nr {0} ({1}).", elemIndexes[elemIndex] + 1, elemStr) + Environment.NewLine + Environment.NewLine;
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
                        Message += "Brak miejsca we wszystkich pudełkach. Dodano nowe pudełko." + Environment.NewLine + Environment.NewLine;
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
                        Message += String.Format("Wylosowano pudełko nr {0}.", index + 1);
                        Wait(index, elemIndexes[elemIndex]);
                    }
                    #endregion
                }

                Result.Bins[index].Insert(elements[elemIndexes[elemIndex]]);

                #region UI
                if (IsPresentation)
                    Message = String.Format("Wstawiono element nr {0} ({1}) do pudełka {2}.", elemIndexes[elemIndex] + 1, elemStr, index + 1) + Environment.NewLine + Environment.NewLine;
                #endregion

                elemIndexes.RemoveAt(elemIndex);
            }

            #region UI
            if (IsPresentation)
                Wait(-1, -1);
            #endregion

            return Result;
        }
    }
}