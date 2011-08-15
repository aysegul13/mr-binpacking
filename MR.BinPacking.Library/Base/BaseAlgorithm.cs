using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPacking.Library.Base
{
    public abstract class BaseAlgorithm
    {
        public virtual string Name { get; protected set; }
        public abstract Instance Execute(List<int> elements, int binSize);
        public virtual Instance Result { get; set; }
    }
}
