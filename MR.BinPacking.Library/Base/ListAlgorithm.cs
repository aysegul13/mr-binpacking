using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MR.BinPacking.Library.Base
{
    public abstract class BaseAlgorithm
    {
        public virtual string Name { get; protected set; }
        public abstract Instance Execute(List<int> elements, int binSize);
        public virtual Instance Result { get; set; }
    }

    public abstract class ListAlgorithm : BaseAlgorithm
    {
        public virtual string Message { get; set; }
        public virtual int PrevSelectedElement { get; set; }
        public virtual int SelectedElement { get; set; }
        public virtual int SelectedBin { get; set; }

        private volatile bool isWaiting = false;
        public virtual bool IsWaiting
        {
            get { return isWaiting; }
            set { isWaiting = value; }
        }

        public virtual bool IsPresentation { get; set; }

        public virtual void Wait(int bin, int elem)
        {
            IsWaiting = true;
            SelectedBin = bin;
            PrevSelectedElement = SelectedElement;
            SelectedElement = elem;

            while (IsWaiting)
                Thread.Sleep(100);
        }
    }
}
