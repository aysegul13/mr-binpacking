using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using System.IO;

namespace MR.BinPacking.App.Utils
{
    internal static class Loader
    {
        public static Instance LoadFromFile(string filename)
        {
            Instance result = new Instance();
            using (StreamReader sr = new StreamReader(filename))
            {
                result.BinSize = Int32.Parse(sr.ReadLine());
                sr.ReadLine();  //line with elements number, ignore

                string line;
                while ((line = sr.ReadLine()) != null)
                    result.Elements.Add(Int32.Parse(line));
            }

            return result;
        }

        public static void SaveToFile(Instance instance, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine(instance.BinSize);
                sw.WriteLine(instance.Elements.Count);

                foreach (var elem in instance.Elements)
                    sw.WriteLine(elem);
            }
        }
    }
}
