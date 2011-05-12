using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPackaging.Library.Base;
using System.IO;

namespace MR.BinPackaging.Test
{
    internal static class Utils
    {
        public static void PrintInstance(Instance instance)
        {
            foreach (var bin in instance.Bins)
            {
                Console.Write("[");

                bool first = true;
                foreach (var elem in bin.Elements)
                {
                    if (!first)
                        Console.Write(", " + elem);
                    else
                        Console.Write(elem);

                    first = false;
                }

                Console.WriteLine("]");
            }
        }

        public static Instance LoadFromFile(string filename)
        {
            Instance result = new Instance();

            using (StreamReader sr = new StreamReader(filename))
            {
                bool first = true;
                int elementsCount = 0;

                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (first)
                    {
                        first = false;

                        result.BinSize = Int32.Parse(line.Substring(0, line.IndexOf(' ')));
                        elementsCount = Int32.Parse(line.Substring(line.IndexOf(' ')));
                    }
                    else
                    {
                        result.Elements.Add(Int32.Parse(line));
                    }
                }
            }

            return result;
        }
    }
}
