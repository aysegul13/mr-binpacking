using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPackaging.Library.Base;
using System.Threading;

namespace MR.BinPackaging.Library
{
    public class FirstFit : IListAlgorithm
    {
        public string Name { get; private set; }

        //presentation properties
        public string Message { get; set; }
        public int SelectedElement { get; set; }
        public int SelectedBin { get; set; }
        public Instance ActualResult { get; set; }

        private volatile bool isWaiting = false;
        public bool IsWaiting
        {
            get
            {
                return isWaiting;
            }
            set
            {
                isWaiting = value;
            }
        }

        public bool IsPresentation { get; set; }

        public FirstFit()
        {
            Name = "First Fit";
            IsWaiting = false;
            IsPresentation = true;
        }

        public void Wait(int bin, int elem)
        {
            IsWaiting = true;
            SelectedBin = bin;
            SelectedElement = elem;

            while (IsWaiting)
                Thread.Sleep(100);
        }

        public Instance Execute(List<int> elements, int binSize)
        {
            if (IsPresentation)
                Message = "";

            ActualResult = new Instance(binSize);
            ActualResult.Elements = elements;

            for (int i = 0; i < ActualResult.Elements.Count; i++)
            {
                int elem = ActualResult.Elements[i];
                bool fit = false;

                for (int k = 0; k < ActualResult.Bins.Count; k++)
                {
                    #region UI
                    if (IsPresentation)
                    {
                        Message += String.Format("Sprawdzanie miejsca w skrzynce {0} dla elementu {1} ({2})", k + 1, i + 1, elem) + Environment.NewLine + Environment.NewLine;
                        Wait(k, i);
                    }
                    #endregion

                    Bin bin = ActualResult.Bins[k];
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
                    ActualResult.Bins.Add(new Bin(ActualResult.BinSize));

                    #region UI
                    if (IsPresentation)
                    {
                        Message += "Brak miejsca we wszystkich skrzynkach. Dodano nową skrzynkę." + Environment.NewLine + Environment.NewLine;
                        Wait(ActualResult.Bins.Count - 1, i);
                    }
                    #endregion

                    ActualResult.Bins.Last().Insert(elem);

                    #region UI
                    if (IsPresentation)
                        Message = String.Format("Wstawiono element {0} ({1}) do skrzynki {2}.", i + 1, elem, ActualResult.Bins.Count) + Environment.NewLine + Environment.NewLine;
                    #endregion
                }
            }

            return ActualResult;
        }
    }
}
