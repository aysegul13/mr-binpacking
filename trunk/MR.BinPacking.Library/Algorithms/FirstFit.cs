using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using System.Threading;

namespace MR.BinPacking.Library.Algorithms
{
    public class FirstFit : ListAlgorithm
    {
        public FirstFit()
        {
            Name = "First Fit";
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
                bool fit = false;

                for (int k = 0; k < Result.Bins.Count; k++)
                {
                    #region UI
                    if (IsPresentation)
                    {
                        Message += String.Format("Sprawdzanie miejsca w skrzynce {0} dla elementu {1} ({2})", k + 1, i + 1, elem) + Environment.NewLine + Environment.NewLine;
                        Wait(k, i);
                    }
                    #endregion

                    Bin bin = Result.Bins[k];
                    if (bin.FreeSpace() >= elem)
                    {
                        fit = true;
                        bin.Insert(elem);

                        #region UI
                        if (IsPresentation)
                            Message = String.Format("Wstawiono element {0} ({1}) do skrzynki {2}.", i + 1, elem, k + 1) + Environment.NewLine + Environment.NewLine;
                        #endregion

                        break;
                    }
                    else
                    {
                        #region UI
                        if (IsPresentation)
                            Message = String.Format("Brak miejsca w skrzynce {0} dla elementu {1} ({2}).", k + 1, i + 1, elem) + Environment.NewLine + Environment.NewLine;
                        #endregion
                    }
                }

                if (!fit)
                {
                    Result.Bins.Add(new Bin(Result.BinSize));

                    #region UI
                    if (IsPresentation)
                    {
                        Message += "Brak miejsca we wszystkich skrzynkach. Dodano nową skrzynkę." + Environment.NewLine + Environment.NewLine;
                        Wait(Result.Bins.Count - 1, i);
                    }
                    #endregion

                    Result.Bins.Last().Insert(elem);

                    #region UI
                    if (IsPresentation)
                        Message = String.Format("Wstawiono element {0} ({1}) do skrzynki {2}.", i + 1, elem, Result.Bins.Count) + Environment.NewLine + Environment.NewLine;
                    #endregion
                }
            }

            return Result;
        }
    }
}
