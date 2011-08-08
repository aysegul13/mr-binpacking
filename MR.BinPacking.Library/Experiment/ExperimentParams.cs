using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using MR.BinPacking.Library.Utils;
using MR.BinPacking.Library.Algorithms;

namespace MR.BinPacking.Library.Experiment
{
    public enum Distribution { Uniform, Gauss, Exponential };
    public enum Sorting { None, Ascending, Descending };
    public enum Algorithm {
        NextFit,
        FirstFit,
        BestFit,
        FirstFitDecreasing,
        BestFitDecreasing,
        RandomFit,
        AsymptoticApproximationScheme,
        BruteForce
    };

    public class ExperimentParamsFile
    {
        public int MinN { get; set; }
        public int MaxN { get; set; }
        public int Step { get; set; }

        public int BinSize { get; set; }
        public double MinVal { get; set; }
        public double MaxVal { get; set; }

        public int Repeat { get; set; }

        public double AASEpsilon { get; set; }

        public List<Algorithm> Algorithms { get; set; }
        public List<Distribution> Distributions { get; set; }
        public List<Sorting> Sortings { get; set; }

        public ExperimentParamsFile()
        {
            Algorithms = new List<Algorithm>();
            Distributions = new List<Distribution>();
            Sortings = new List<Sorting>();
        }
    }

    public class ExperimentParams : ExperimentParamsFile
    {
        public List<BaseAlgorithm> Algs { get; set; }

        public ExperimentParams()
        {
            Algs = new List<BaseAlgorithm>();
        }

        public ExperimentParams(ExperimentParamsFile fileParams) : this()
        {
            MinN = fileParams.MinN;
            MaxN = fileParams.MaxN;
            Step = fileParams.Step;
            BinSize = fileParams.BinSize;
            MinVal = fileParams.MinVal;
            MaxVal = fileParams.MaxVal;
            Repeat = fileParams.Repeat;
            AASEpsilon = fileParams.AASEpsilon;
            Algorithms = fileParams.Algorithms;
            Distributions = fileParams.Distributions;
            Sortings = fileParams.Sortings;

            foreach (var algorithm in fileParams.Algorithms)
                Algs.Add(ListAlgorithmFromAlgorithm(algorithm, AASEpsilon));
        }

        static BaseAlgorithm ListAlgorithmFromAlgorithm(Algorithm algorithm, double AASEpsilon)
        {
            bool isPresentation = false;

            switch (algorithm)
            {
                case Algorithm.FirstFit:
                    return new FirstFit() { IsPresentation = isPresentation };
                case Algorithm.BestFit:
                    return new BestFit() { IsPresentation = isPresentation };
                case Algorithm.FirstFitDecreasing:
                    return new FirstFitDecreasing() { IsPresentation = isPresentation };
                case Algorithm.BestFitDecreasing:
                    return new BestFitDecreasing() { IsPresentation = isPresentation };
                case Algorithm.RandomFit:
                    return new RandomFit() { IsPresentation = isPresentation };
                case Algorithm.AsymptoticApproximationScheme:
                    return new AAS() { Epsilon = AASEpsilon };
                case Algorithm.BruteForce:
                    throw new NotImplementedException();
                    //return new NextFit() { IsPresentation = isPresentation };
                default:    //NextFit
                    return new NextFit() { IsPresentation = isPresentation };
            }
        }
    }
}
