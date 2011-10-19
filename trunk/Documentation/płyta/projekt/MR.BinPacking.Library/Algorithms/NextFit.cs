using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using System.Threading;

namespace MR.BinPacking.Library.Algorithms
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

            Result = new Instance(binSize);
            Result.Elements = elements;

            Result.Bins.Add(new Bin(Result.BinSize));

            if (IsPresentation)
                Message = "Brak pudełek. Dodano nowe pudełko." + Environment.NewLine + Environment.NewLine;

            int k = 0;
            int sum = 0;

            for (int i = 0; i < Result.Elements.Count; i++)
            {
                int elem = Result.Elements[i];
                string elemStr = ShowScaled ? ((double)elem / binSize).ToString("0.####") : elem.ToString();

                #region UI
                if (IsPresentation)
                {
                    Message += String.Format("Sprawdzanie miejsca w pudełku {0} dla elementu nr {1} ({2})", k + 1, i + 1, elemStr) + Environment.NewLine + Environment.NewLine;
                    Wait(k, i);
                }
                #endregion

                if (sum + elem > Result.BinSize)
                {
                    Result.Bins.Add(new Bin(Result.BinSize));
                    k++;
                    sum = 0;

                    #region UI
                    if (IsPresentation)
                    {
                        Message = String.Format("Brak miejsca w pudełku {0} dla elementu nr {1} ({2}). Dodano nowe pudełko.", k, i + 1, elemStr) + Environment.NewLine + Environment.NewLine;
                        Wait(k, i);
                    }
                    #endregion
                }

                Result.Bins[k].Insert(elem);
                sum += elem;

                #region UI
                if (IsPresentation)
                    Message = String.Format("Wstawiono element nr {0} ({1}) do pudełka {2}.", i + 1, elemStr, k + 1) + Environment.NewLine + Environment.NewLine;
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
