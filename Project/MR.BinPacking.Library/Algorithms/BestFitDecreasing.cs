using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using System.Threading;

namespace MR.BinPacking.Library.Algorithms
{
    public class BestFitDecreasing : ListAlgorithm
    {
        public override string Message
        {
            get
            {
                if (algorithm != null)
                    return algorithm.Message;
                else
                    return base.Message;
            }
            set
            {
                base.Message = value;
            }
        }

        public override int PrevSelectedElement
        {
            get
            {
                if (algorithm != null)
                    return algorithm.PrevSelectedElement;
                else
                    return base.PrevSelectedElement;
            }
            set
            {
                base.PrevSelectedElement = value;
            }
        }

        public override int SelectedElement
        {
            get
            {
                if (algorithm != null)
                    return algorithm.SelectedElement;
                else
                    return base.SelectedElement;
            }
            set
            {
                base.SelectedElement = value;
            }
        }

        public override int SelectedBin
        {
            get
            {
                if (algorithm != null)
                    return algorithm.SelectedBin;
                else
                    return base.SelectedBin;
            }
            set
            {
                base.SelectedBin = value;
            }
        }

        public override Instance Result
        {
            get
            {
                if (algorithm != null)
                    return algorithm.Result;
                else
                    return base.Result;
            }
            set
            {
                base.Result = value;
            }
        }

        public override bool IsWaiting
        {
            get
            {
                return base.IsWaiting;
            }
            set
            {
                base.IsWaiting = value;
                if (algorithm != null)
                    algorithm.IsWaiting = value;
            }
        }

        public override bool IsPresentation
        {
            get
            {
                return base.IsPresentation;
            }
            set
            {
                base.IsPresentation = value;
                if (algorithm != null)
                    algorithm.IsPresentation = value;
            }
        }

        private ListAlgorithm algorithm = null;

        public BestFitDecreasing()
        {
            Name = "Best Fit Decreasing";
            IsWaiting = false;
            IsPresentation = true;
        }

        public override void Wait(int bin, int elem)
        {
            IsWaiting = true;
            SelectedBin = bin;
            PrevSelectedElement = SelectedElement;
            SelectedElement = elem;

            if (algorithm != null)
            {
                while (algorithm.IsWaiting)
                    Thread.Sleep(100);
            }
            else
            {
                while (IsWaiting)
                    Thread.Sleep(100);
            }
        }

        public override Instance Execute(List<int> elements, int binSize)
        {
            if (IsPresentation)
                Message = "";

            Result = new Instance(binSize);
            Result.Elements = elements;

            List<int> elementsSorted = new List<int>(elements);
            elementsSorted.Sort((x, y) => y.CompareTo(x));

            Result.Elements = elementsSorted;

            if (IsPresentation)
            {
                Message = "Elementy zostały posortowane malejąco." + Environment.NewLine + Environment.NewLine;
                Wait(-1, -1);
            }

            algorithm = new BestFit();
            algorithm.IsPresentation = IsPresentation;

            return algorithm.Execute(elementsSorted, binSize);
        }
    }
}
