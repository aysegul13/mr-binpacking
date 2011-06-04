using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPacking.Library.Base
{
    public class Instance
    {
        public List<int> Elements { get; set; }
        public List<Bin> Bins { get; set; }

        public int BinSize { get; set; }


        public Instance(int binSize)
        {
            Elements = new List<int>();
            Bins = new List<Bin>();
            BinSize = binSize;
        }

        public Instance() : this(10) { }
    }
}
