using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MR.BinPacking.Library.Base;

namespace MR.BinPacking.App.Utils
{
    internal static class ESLoader
    {
        public static Instance LoadFromFile1(string filename, int elemCountIdx, int binSizeIdx)
        {
            Instance result = new Instance();

            using (StreamReader sr = new StreamReader(filename))
            {
                string line;
                List<int> numbers = new List<int>();

                while ((line = sr.ReadLine()) != null)
                    numbers.Add(Int32.Parse(line.Trim()));

                int skip = 0;

                if (binSizeIdx >= 0)
                {
                    result.BinSize = numbers[binSizeIdx];
                    skip++;
                }

                int elemCount;
                if (elemCountIdx >= 0)
                {
                    elemCount = numbers[elemCountIdx];
                    skip++;
                }
                else
                {
                    elemCount = numbers.Count - skip;
                }

                result.Elements = numbers.Skip(skip).Take(elemCount).ToList();
            }

            return result;
        }

        public static List<Instance> LoadFromFile2(string filename)
        {
            List<Instance> result = new List<Instance>();

            using (StreamReader sr = new StreamReader(filename))
            {
                string line = sr.ReadLine();
                if (line == null)
                    return result;
                int instancesCount = Int32.Parse(line.Trim());

                while (true)
                {
                    line = sr.ReadLine();
                    if (line == null)
                        return result;
                    string name = line;

                    line = sr.ReadLine();
                    if (line == null)
                        return result;
                    string[] instanceInfo = line.Trim().Split().Where(ei => !String.IsNullOrEmpty(ei)).ToArray();

                    int binSize = Int32.Parse(instanceInfo[0]);
                    int elemCount = Int32.Parse(instanceInfo[1]);
                    //int bestKnownSolution = Int32.Parse(instanceInfo[2]);

                    Instance instance = new Instance(binSize) { Name = name };

                    for (int i = 0; i < elemCount; i++)
                    {
                        line = sr.ReadLine();
                        if (line == null)
                            return result;

                        int elemSize = Int32.Parse(line.Trim());
                        instance.Elements.Add(elemSize);
                    }

                    result.Add(instance);
                }
            }
        }

        public static List<Instance> LoadFromFile3(string filename)
        {
            List<Instance> result = new List<Instance>();

            using (StreamReader sr = new StreamReader(filename))
            {
                while (true)
                {
                    string line = sr.ReadLine();
                    if (line == null)
                        return result;
                    string name = line;

                    line = sr.ReadLine();
                    if (line == null)
                        return result;
                    int weightsCount = Int32.Parse(line.Trim());

                    line = sr.ReadLine();
                    if (line == null)
                        return result;
                    int binSize = Int32.Parse(line.Trim());

                    Instance instance = new Instance(binSize) { Name = name };

                    for (int i = 0; i < weightsCount; i++)
                    {
                        line = sr.ReadLine();
                        if (line == null)
                            return result;
                        string[] elemInfo = line.Trim().Split();


                        int elemSize = Int32.Parse(elemInfo.Where(ei => !String.IsNullOrEmpty(ei)).ElementAt(0));
                        int elemCount = Int32.Parse(elemInfo.Where(ei => !String.IsNullOrEmpty(ei)).ElementAt(1));

                        for (int j = 0; j < elemCount; j++)
                            instance.Elements.Add(elemSize);
                    }

                    result.Add(instance);
                }
            }
        }
    }
}
