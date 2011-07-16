using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using System.IO;
using MR.BinPacking.Library.Experiment;
using System.Xml.Serialization;

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

        public static ExperimentParamsFile LoadExperimentParams(string filename)
        {
            string xml = null;
            using (StreamReader sr = new StreamReader(filename))
                xml = sr.ReadToEnd();

            XmlSerializer serializer = new XmlSerializer(typeof(ExperimentParamsFile));
            StringReader stringReader = new StringReader(xml);

            ExperimentParamsFile experimentParams = serializer.Deserialize(stringReader) as ExperimentParamsFile;
            stringReader.Close();

            return experimentParams;
        }

        public static void SaveExperimentParams(ExperimentParamsFile experimentParams, string fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ExperimentParamsFile));
            MemoryStream ms = new MemoryStream();

            serializer.Serialize(ms, experimentParams);

            byte[] bytes = ms.ToArray();
            ms.Close();

            string xml = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            using (StreamWriter sw = new StreamWriter(fileName))
                sw.Write(xml);
        }
    }
}
