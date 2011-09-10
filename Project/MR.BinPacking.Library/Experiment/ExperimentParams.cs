using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MR.BinPacking.Library.Base;
using MR.BinPacking.Library.Utils;
using MR.BinPacking.Library.Algorithms;

namespace MR.BinPacking.Library.Experiment
{
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
                Algs.Add(BaseAlgorithmFromAlgorithm(algorithm, AASEpsilon));
        }

        static BaseAlgorithm BaseAlgorithmFromAlgorithm(Algorithm algorithm, double AASEpsilon)
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
                case Algorithm.Reduction:
                    return new Reduction();
                case Algorithm.Exact:
                    return new Exact();
                case Algorithm.PBI:
                    return new PBI();
                default:    //NextFit
                    return new NextFit() { IsPresentation = isPresentation };
            }
        }
    }
}
