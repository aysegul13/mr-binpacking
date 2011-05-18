using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPackaging.Library.Base
{
    public interface IListAlgorithm
    {
        string Name { get; }

        //presentation properties
        string Message { get; set; }
        int SelectedElement { get; set; }
        int SelectedBin { get; set; }
        bool IsWaiting { get; set; }
        Instance ActualResult { get; set; }

        Instance Execute(List<int> elements, int binSize);
    }
}
