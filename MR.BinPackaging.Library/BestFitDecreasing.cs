using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPackaging.Library.Base;
using System.Threading;

namespace MR.BinPackaging.Library
{
    public class BestFitDecreasing : IListAlgorithm
    {
        public string Name { get; private set; }

        //presentation properties
        private string message;
        public string Message
        {
            get
            {
                if (algorithm != null)
                    return algorithm.Message;
                else
                    return message;
            }
            set
            {
                message = value;
            }
        }

        private int selectedElement;
        public int SelectedElement
        {
            get
            {
                if (algorithm != null)
                    return algorithm.SelectedElement;
                else
                    return selectedElement;
            }
            set
            {
                selectedElement = value;
            }
        }

        private int selectedBin;
        public int SelectedBin
        {
            get
            {
                if (algorithm != null)
                    return algorithm.SelectedBin;
                else
                    return selectedBin;
            }
            set
            {
                selectedBin = value;
            }
        }

        private Instance actualResult = null;
        public Instance ActualResult
        {
            get
            {
                if (algorithm != null)
                    return algorithm.ActualResult;
                else
                    return actualResult;
            }
            set
            {
                actualResult = value;
            }
        }

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
                if (algorithm != null)
                    algorithm.IsWaiting = value;
            }
        }

        public bool IsPresentation { get; set; }

        private IListAlgorithm algorithm = null;

        public BestFitDecreasing()
        {
            Name = "Best Fit Decreasing";
            IsWaiting = false;
            IsPresentation = true;
        }

        public void Wait(int bin, int elem)
        {
            IsWaiting = true;
            SelectedBin = bin;
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

        public Instance Execute(List<int> elements, int binSize)
        {
            if (IsPresentation)
                Message = "";

            ActualResult = new Instance(binSize);
            ActualResult.Elements = elements;

            List<int> elementsSorted = new List<int>(elements);
            elementsSorted.Sort((x, y) => y.CompareTo(x));

            ActualResult.Elements = elementsSorted;

            if (IsPresentation)
            {
                Message = "Elementy zostały posortowane malejąco." + Environment.NewLine + Environment.NewLine;
                Wait(-1, -1);
            }

            algorithm = new BestFit();

            return algorithm.Execute(elementsSorted, binSize);
        }
    }
}
