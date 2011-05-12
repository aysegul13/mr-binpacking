using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPackaging.Library.Base;
using System.IO;

namespace MR.BinPackaging.App.Utils
{
    internal static class Loader
    {
        //TODO: używać tej metody :P
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

        //public static void Shuffle<T>(this IList<T> list)
        //{
        //    var provider = new RNGCryptoServiceProvider();
        //    int n = list.Count;
        //    while (n > 1)
        //    {
        //        var box = new byte[1];
        //        do provider.GetBytes(box);
        //        while (!(box[0] < n * (Byte.MaxValue / n)));
        //        var k = (box[0] % n);
        //        n--;
        //        var value = list[k];
        //        list[k] = list[n];
        //        list[n] = value;
        //    }
        //}  
    }
}
