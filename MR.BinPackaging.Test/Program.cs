using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MR.BinPackaging.Library.Base;
using MR.BinPackaging.Library;
using System.Threading;

namespace MR.BinPackaging.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            ExperimentParams prms = new ExperimentParams()
            {
                Algorithm = new NextFit(),
                BinSize = 100,
                Dist = Distribution.Uniform,
                MinN = 100,
                MaxN = 10000,
                Step = 100,
                MinVal = 0.0,
                MaxVal = 1.0,
                Repeat = 10
            };

            List<List<Statistics>> stats = Experiment.ExecuteExperiment(prms);

            List<Point2D> results = Experiment.GetCoordinates(stats, StatField.Result);


            int a = 10;

            //Instance instance = Utils.LoadFromFile(@"datasets/u250_12.txt");
            //Instance result = FirstFitDecreasing.Execute(instance.Elements, instance.BinSize);


            //Instance instance = new Instance();
            List<int> elements = new List<int>() { 5, 8, 3, 5, 1, 2, 9, 7 };
            //List<int> elements = new List<int>() { 5, 6, 2, 3, 4 };
            //List<int> elements = new List<int>() { 5, 6, 4, 2, 3 };

            //Console.WriteLine("NextFit");
            //Instance result = NextFit.Execute(elements, 10);
            //Utils.PrintInstance(result);
            //Console.WriteLine();

            //Console.WriteLine("FirstFit");
            //Instance result = FirstFit.Execute(elements, 10);
            //Utils.PrintInstance(result);
            //Console.WriteLine();

            //Console.WriteLine("BestFit");
            //Instance result = BestFit.Execute(elements, 10);
            //Utils.PrintInstance(result);
            //Console.WriteLine();

            Console.WriteLine("L_1:");
            Console.WriteLine(Bounds.LowerBound(elements, 10));
            Console.WriteLine();

            Console.WriteLine("L_1:");
            Console.WriteLine(Bounds.StrongerLowerBound(elements, 10, 4));
            Console.WriteLine();

            ////elements = new List<int>() { 70, 40, 40, 15, 15, 10, 10 };
            //elements = new List<int>() { 70, 40, 40, 20, 15, 15 };

            //Console.WriteLine("FirstFitDecreasing");
            //result = FirstFitDecreasing.Execute(elements, 100);
            //Utils.PrintInstance(result);
            //Console.WriteLine();

            //Console.WriteLine("BestFitDecreasing");
            //result = BestFitDecreasing.Execute(elements, 100);
            //Utils.PrintInstance(result);
            //Console.WriteLine();


            #region random
            const int RANGE_COUNT = 20;
            const int COUNT = 1000000;
            Random rand = new Random();

            List<double> vals = new List<double>();

            for (int i = 0; i < COUNT; i++)
                vals.Add(Probability.RandomExponential(rand));

            double min = vals.Min();
            double max = vals.Max();

            Console.WriteLine(min);
            Console.WriteLine(max);

            //double width = (max - min) / RANGE_COUNT;
            //int[] test = new int[RANGE_COUNT];

            //for (int i = 0; i < COUNT; i++)
            //{
            //    for (int j = 0; j < RANGE_COUNT; j++)
            //    {
            //        double rangeMax = min + (j + 1) * width;
            //        if (vals[i] <= rangeMax)
            //        {
            //            test[j] += 1;
            //            break;
            //        }
            //    }
            //}

            //for (int i = 0; i < RANGE_COUNT; i++)
            //    Console.WriteLine(String.Format("[{0}]", test[i]));
            #endregion

            Console.ReadKey();
        }
    }
}
