using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using System.Threading;

namespace MR.BinPacking.Library.Algorithms
{
    public class BestFit : ListAlgorithm
    {
        public BestFit()
        {
            Name = "Best Fit";
            IsWaiting = false;
            IsPresentation = true;
        }

        public override Instance Execute(List<int> elements, int binSize)
        {
            if (IsPresentation)
                Message = "";

            Result = new Instance(binSize);
            Result.Elements = elements;

            for (int i = 0; i < Result.Elements.Count; i++)
            {
                int elem = Result.Elements[i];
                string elemStr = ShowScaled ? ((double)elem / binSize).ToString("0.####") : elem.ToString();
                int minSpaceLeft = binSize;
                int minIndex = -1;

                for (int k = 0; k < Result.Bins.Count; k++)
                {
                    #region UI
                    if (IsPresentation)
                    {
                        Message += String.Format("Sprawdzanie miejsca w skrzynce {0} dla elementu nr {1} ({2})", k + 1, i + 1, elemStr) + Environment.NewLine + Environment.NewLine;
                        Wait(k, i);
                    }
                    #endregion

                    if ((Result.Bins[k].FreeSpace() >= elem) && (Result.Bins[k].FreeSpace() - elem < minSpaceLeft))
                    {
                        minSpaceLeft = Result.Bins[k].FreeSpace() - elem;
                        minIndex = k;

                        #region UI
                        if (IsPresentation)
                        {
                            Message = String.Format("Znaleziono lepsze dopasowanie - skrzynka {0}", k + 1) + Environment.NewLine + Environment.NewLine;
                            Wait(k, i);
                        }
                        #endregion
                    }
                    else
                    {
                        #region UI
                        if (IsPresentation)
                        {
                            if (Result.Bins[k].FreeSpace() < elem)
                                Message = String.Format("Brak miejsca w skrzynce {0} dla elementu nr {1} ({2}).", k + 1, i + 1, elemStr) + Environment.NewLine + Environment.NewLine;
                            else
                                Message = "Aktualne dopasowanie jest gorsze od najlepszego znalezionego." + Environment.NewLine + Environment.NewLine;
                        }
                        #endregion
                    }
                }

                if (minIndex < 0)
                {
                    Result.Bins.Add(new Bin(Result.BinSize));
                    minIndex = Result.Bins.Count - 1;

                    #region UI
                    if (IsPresentation)
                    {
                        Message += "Brak miejsca we wszystkich skrzynkach. Dodano nową skrzynkę." + Environment.NewLine + Environment.NewLine;
                        Wait(minIndex, i);
                    }
                    #endregion
                }

                Result.Bins[minIndex].Insert(elem);

                #region UI
                if (IsPresentation)
                    Message = String.Format("Wstawiono element nr {0} ({1}) do skrzynki {2}.", i + 1, elemStr, minIndex + 1) + Environment.NewLine + Environment.NewLine;
                #endregion
            }

            #region UI
            if (IsPresentation)
                Wait(-1, -1);
            #endregion

            return Result;
        }
    }
}
