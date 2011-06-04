using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MR.BinPacking.Library.Base
{
    public abstract class ListAlgorithm
    {
        public virtual string Name { get; protected set; }

        public virtual string Message { get; set; }
        public virtual int SelectedElement { get; set; }
        public virtual int SelectedBin { get; set; }

        private volatile bool isWaiting = false;
        public virtual bool IsWaiting
        {
            get { return isWaiting; }
            set { isWaiting = value; }
        }

        public bool IsPresentation { get; set; }
        public virtual Instance ActualResult { get; set; }

        public virtual void Wait(int bin, int elem)
        {
            IsWaiting = true;
            SelectedBin = bin;
            SelectedElement = elem;

            while (IsWaiting)
                Thread.Sleep(100);
        }

        public abstract Instance Execute(List<int> elements, int binSize);
    }
}
