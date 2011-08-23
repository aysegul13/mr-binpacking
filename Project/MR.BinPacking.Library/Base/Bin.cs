using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPacking.Library.Base
{
    public class Bin
    {
        private List<int> elements = new List<int>();
        public List<int> Elements
        {
            get
            {
                return elements;
            }
        }

        public int Size { get; set; }


        public Bin(int size)
        {
            Size = size;
        }

        public Bin() : this(10) { }


        public int FreeSpace()
        {
            return Size - Elements.Sum();
        }

        public void Insert(int element)
        {
            elements.Add(element);
        }
    }
}
