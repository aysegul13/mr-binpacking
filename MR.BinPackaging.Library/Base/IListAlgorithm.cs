using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPackaging.Library.Base
{
    public interface IListAlgorithm
    {
        string Name { get; }

        Instance Execute(List<int> elements, int binSize);
    }
}
