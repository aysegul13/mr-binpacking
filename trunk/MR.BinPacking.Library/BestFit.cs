using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPackaging.Library.Base;
using System.Threading;

namespace MR.BinPackaging.Library
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

            ActualResult = new Instance(binSize);
            ActualResult.Elements = elements;

            for (int i = 0; i < ActualResult.Elements.Count; i++)
            {
                int elem = ActualResult.Elements[i];
                int minSpaceLeft = binSize;
                int minIndex = -1;

                for (int k = 0; k < ActualResult.Bins.Count; k++)
                {
                    #region UI
                    if (IsPresentation)
                    {
                        Message += String.Format("Sprawdzanie miejsca w skrzynce {0} dla elementu {1} ({2})", k + 1, i + 1, elem) + Environment.NewLine + Environment.NewLine;
                        Wait(k, i);
                    }
                    #endregion

                    if ((ActualResult.Bins[k].FreeSpace() >= elem) && (ActualResult.Bins[k].FreeSpace() - elem < minSpaceLeft))
                    {
                        minSpaceLeft = ActualResult.Bins[k].FreeSpace() - elem;
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
                            if (ActualResult.Bins[k].FreeSpace() < elem)
                                Message = String.Format("Brak miejsca w skrzynce {0} dla elementu {1} ({2}).", k + 1, i + 1, elem) + Environment.NewLine + Environment.NewLine;
                            else
                                Message = "Aktualne dopasowanie jest gorsze od najlepszego znalezionego." + Environment.NewLine + Environment.NewLine;
                        }
                        #endregion
                    }
                }

                if (minIndex < 0)
                {
                    ActualResult.Bins.Add(new Bin(ActualResult.BinSize));
                    minIndex = ActualResult.Bins.Count - 1;

                    #region UI
                    if (IsPresentation)
                    {
                        Message += "Brak miejsca we wszystkich skrzynkach. Dodano nową skrzynkę." + Environment.NewLine + Environment.NewLine;
                        Wait(minIndex, i);
                    }
                    #endregion
                }

                ActualResult.Bins[minIndex].Insert(elem);

                #region UI
                if (IsPresentation)
                    Message = String.Format("Wstawiono element {0} ({1}) do skrzynki {2}.", i + 1, elem, minIndex + 1) + Environment.NewLine + Environment.NewLine;
                #endregion
            }

            return ActualResult;
        }
    }
}
