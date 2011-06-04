using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using System.Threading;

namespace MR.BinPacking.Library
{
    public class NextFit : ListAlgorithm
    {
        public NextFit()
        {
            Name = "Next Fit";
            IsWaiting = false;

            IsPresentation = true;
        }

        public override Instance Execute(List<int> elements, int binSize)
        {
            if (IsPresentation)
                Message = "";

            ActualResult = new Instance(binSize);
            ActualResult.Elements = elements;

            ActualResult.Bins.Add(new Bin(ActualResult.BinSize));

            if (IsPresentation)
                Message = "Brak skrzynek. Dodano nową skrzynkę." + Environment.NewLine + Environment.NewLine;

            int k = 0;
            int sum = 0;

            for (int i = 0; i < ActualResult.Elements.Count; i++)
            {
                int elem = ActualResult.Elements[i];

                #region UI
                if (IsPresentation)
                {
                    Message += String.Format("Sprawdzanie miejsca w skrzynce {0} dla elementu {1} ({2})", k + 1, i + 1, elem) + Environment.NewLine + Environment.NewLine;
                    Wait(k, i);
                }
                #endregion

                if (sum + elem > ActualResult.BinSize)
                {
                    ActualResult.Bins.Add(new Bin(ActualResult.BinSize));
                    k++;
                    sum = 0;

                    #region UI
                    if (IsPresentation)
                    {
                        Message = String.Format("Brak miejsca w skrzynce {0} dla elementu {1} ({2}). Dodano nową skrzynkę.", k, i + 1, elem) + Environment.NewLine + Environment.NewLine;
                        Wait(k, i);
                    }
                    #endregion
                }

                ActualResult.Bins[k].Insert(elem);
                sum += elem;

                #region UI
                if (IsPresentation)
                    Message = String.Format("Wstawiono element {0} ({1}) do skrzynki {2}.", i + 1, elem, k + 1) + Environment.NewLine + Environment.NewLine;
                #endregion
            }

            return ActualResult;
        }
    }
}
