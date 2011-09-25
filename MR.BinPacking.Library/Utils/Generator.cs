﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Experiment;
using MR.BinPacking.Library.Base;

namespace MR.BinPacking.Library.Utils
{
    public static class Generator
    {
        public static List<ExperimentInstance> GenerateInstances(ExperimentParams prms)
        {
            List<ExperimentInstance> result = new List<ExperimentInstance>();

            for (int R = 0; R < prms.Repeat; R++)
            {
                int N = prms.MinN;
                while (N <= prms.MaxN)
                {
                    int min = (int)Math.Ceiling(prms.MinVal * prms.BinSize);
                    int max = (int)Math.Ceiling(prms.MaxVal * prms.BinSize);

                    foreach (var D in prms.Distributions)
                    {
                        result.Add(new ExperimentInstance()
                        {
                            BinSize = prms.BinSize,
                            Dist = D,
                            Elements = Generator.GenerateData(N, min, max, D)
                        });
                    }

                    N += prms.Step;
                }
            }

            return result;
        }

        public static List<int> GenerateData(int n, int min, int max, Distribution distribution)
        {
            switch (distribution)
            {
                case Distribution.Gauss:
                    return GetRandomGaussData(n, min, max);
                case Distribution.Exponential:
                    return GetRandomExponentialData(n, min, max);
                default:
                    return GetRandomUniformData(n, min, max);
            }
        }

        private static List<int> GetRandomUniformData(int n, int min, int max)
        {
            if (min < 1)
                min = 1;

            List<int> result = new List<int>();
            Random random = new Random();

            for (int i = 0; i < n; i++)
            {
                int newElem = min + (int)Math.Round(Probability.RandomUniform(random) * (max - min));
                result.Add(newElem);
            }

            return result;
        }

        private static List<int> GetRandomGaussData(int n, int min, int max)
        {
            List<int> result = new List<int>();
            Random random = new Random();

            List<double> tmpElements = new List<double>();
            for (int i = 0; i < n; i++)
                tmpElements.Add(Probability.RandomGauss(random));

            double minValue = tmpElements.Min();
            double maxValue = tmpElements.Max();

            for (int i = 0; i < n; i++)
            {
                double val = (max - min) * (tmpElements[i] - minValue);

                int newElem;
                if (minValue != maxValue)
                    newElem = min + (int)Math.Round(val / (maxValue - minValue));
                else
                    newElem = min + (int)Math.Round(val);

                result.Add(newElem);
            }

            return result;
        }

        private static List<int> GetRandomExponentialData(int n, int min, int max)
        {
            List<int> result = new List<int>();
            Random random = new Random();

            List<double> tmpElements = new List<double>();
            for (int i = 0; i < n; i++)
                tmpElements.Add(Probability.RandomExponential(random));

            double maxValue = tmpElements.Max(); //min is 0.0

            for (int i = 0; i < n; i++)
            {
                int newElem = min + (int)Math.Round((max - min) * tmpElements[i] / maxValue);
                result.Add(newElem);
            }

            return result;
        }
    }
}