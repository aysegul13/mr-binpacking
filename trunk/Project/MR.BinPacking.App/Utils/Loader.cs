using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using System.IO;
using MR.BinPacking.Library.Experiment;
using System.Xml.Serialization;
using System.Windows.Media.Imaging;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Media;

namespace MR.BinPacking.App.Utils
{
    public enum FileType { Simple, Multi, MultiWithWeights };

    internal static class Loader
    {
        public static ExperimentInstance LoadInstance1(string filename, int elemCountIdx, int binSizeIdx, int binSize)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                string line;
                List<int> numbers = new List<int>();

                while ((line = sr.ReadLine()) != null)
                    numbers.Add(Int32.Parse(line.Trim()));

                ExperimentInstance result = new ExperimentInstance()
                {
                    Dist = Distribution.None,
                    BinSize = binSize
                };

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

                return result;
            }
        }

        public static List<ExperimentInstance> LoadInstance2(string filename)
        {
            List<ExperimentInstance> result = new List<ExperimentInstance>();

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

                    ExperimentInstance instance = new ExperimentInstance(binSize)
                    {
                        Name = name,
                        Dist = Distribution.None
                    };

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

        public static List<ExperimentInstance> LoadInstance3(string filename)
        {
            List<ExperimentInstance> result = new List<ExperimentInstance>();

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

                    ExperimentInstance instance = new ExperimentInstance(binSize)
                    {
                        Name = name,
                        Dist = Distribution.None
                    };

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

        public static void SaveInstance(Instance instance, string filename)
        {
            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine(instance.BinSize);
                sw.WriteLine(instance.Elements.Count);

                foreach (var elem in instance.Elements)
                    sw.WriteLine(elem);
            }
        }


        //public static Settings LoadSettings(string filename)
        //{
        //    try
        //    {
        //        string xml = null;
        //        using (StreamReader sr = new StreamReader(filename))
        //            xml = sr.ReadToEnd();

        //        XmlSerializer serializer = new XmlSerializer(typeof(Settings));
        //        StringReader stringReader = new StringReader(xml);

        //        Settings settings = serializer.Deserialize(stringReader) as Settings;
        //        stringReader.Close();

        //        return settings;
        //    }
        //    catch
        //    {
        //        return new Settings();
        //    }
        //}

        //public static void SaveSettings(Settings settings, string filename)
        //{
        //    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
        //    MemoryStream ms = new MemoryStream();

        //    serializer.Serialize(ms, settings);

        //    byte[] bytes = ms.ToArray();
        //    ms.Close();

        //    string xml = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        //    using (StreamWriter sw = new StreamWriter(filename))
        //        sw.Write(xml);
        //}


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

        public static void SaveExperimentParams(ExperimentParamsFile experimentParams, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ExperimentParamsFile));
            MemoryStream ms = new MemoryStream();

            serializer.Serialize(ms, experimentParams);

            byte[] bytes = ms.ToArray();
            ms.Close();

            string xml = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            using (StreamWriter sw = new StreamWriter(filename))
                sw.Write(xml);
        }

        
        public static void SaveControlImage(UIElement control, double width, double height)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".png";
            dialog.Filter = "Bitmapa (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg|Pliki PNG (*.png)|*.png|Obrazki (*.bmp;*.jpg;*.png)|*.bmp;*.jpg;*.png|Wszystkie pliki (*.*)|*.*";
            dialog.FilterIndex = 3;

            int Width = (int)Math.Floor(width);
            int Height = (int)Math.Floor(height);

            Nullable<bool> result = dialog.ShowDialog();
            if (result != true)
                return;

            string file = dialog.FileName;
            string Extension = System.IO.Path.GetExtension(file).ToLower();


            RenderTargetBitmap bmp = new RenderTargetBitmap(Width, Height, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(control);

            BitmapEncoder encoder;

            if (Extension == ".bmp")
                encoder = new BmpBitmapEncoder();
            else if (Extension == ".png")
                encoder = new PngBitmapEncoder();
            else if ((Extension == ".jpg") || (Extension == ".jpeg"))
            {
                encoder = new JpegBitmapEncoder();
                (encoder as JpegBitmapEncoder).QualityLevel = 100;
            }
            else
                return;

            encoder.Frames.Add(BitmapFrame.Create(bmp));

            using (Stream stm = File.Create(file))
            {
                encoder.Save(stm);
            }
        }
    }
}
