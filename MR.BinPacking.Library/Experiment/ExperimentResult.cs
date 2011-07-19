using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MR.BinPacking.Library.Experiment
{
    public enum ChartDataType
    {
        Algorithm,
        Distribution,
        Sorting,
        AlgorithmDistribution,
        AlgorithmSorting,
        DistributionSorting
    };

    public class ExperimentResult
    {
        public ExperimentParams Params { get; set; }
        public List<Sample> Samples { get; set; }

        //public List<AlgorithmResult> DataSeries { get; set; }

        //public ExperimentResult()
        //{
        //    DataSeries = new List<AlgorithmResult>();
        //}

        //public AlgorithmResult this[int index]
        //{
        //    get { return DataSeries[index]; }
        //    set { DataSeries[index] = value; }
        //}

        private Sample GetAvg(IGrouping<int, Sample> group)
        {
            Sample first = group.First();

            Sample avgSample = new Sample()
            {
                ID = first.ID,
                Algorithm = first.Algorithm,
                Distribution = first.Distribution,
                N = first.N,
                Sorting = first.Sorting,
                ExecutionTime = group.Sum(s => s.ExecutionTime) / group.Count(),
                LowerBound = (int)Math.Ceiling(group.Average(s => s.LowerBound)),
                Result = (int)Math.Ceiling(group.Average(s => s.Result)),
                StrongerLowerBound = (int)Math.Ceiling(group.Average(s => s.StrongerLowerBound))
            };

            return avgSample;
        }

        public IEnumerable<Sample> GetAvgSamples()
        {
            return from s in Samples
                   group s by s.ID % (Samples.Count / Params.Repeat) into g
                   select GetAvg(g);
        }

        private object GetGroupingKey(Sample sample, ChartDataType type)
        {
            switch (type)
            {
                case ChartDataType.Distribution:
                    return sample.Distribution;
                case ChartDataType.Sorting:
                    return sample.Sorting;
                case ChartDataType.AlgorithmDistribution:
                    return new { sample.Algorithm, sample.Distribution };
                case ChartDataType.AlgorithmSorting:
                    return new { sample.Algorithm, sample.Sorting };
                case ChartDataType.DistributionSorting:
                    return new { sample.Distribution, sample.Sorting };
                default:
                    return sample.Algorithm;
            }
        }

        private string GetSerieName(IEnumerable<Sample> serie, ChartDataType type)
        {
            Sample first = serie.First();
            switch (type)
            {
                case ChartDataType.Distribution:
                    return first.Distribution.ToString();
                case ChartDataType.Sorting:
                    return first.Sorting.ToString();
                case ChartDataType.AlgorithmDistribution:
                    return first.Algorithm.ToString() + ", " + first.Distribution.ToString();
                case ChartDataType.AlgorithmSorting:
                    return first.Algorithm.ToString() + ", " + first.Sorting.ToString();
                case ChartDataType.DistributionSorting:
                    return first.Distribution.ToString() + ", " + first.Sorting.ToString();
                default:
                    return first.Algorithm.ToString();
            }
        }

        //public IEnumerable<IGrouping<object, Sample>> GetDataSeries(ChartType type)
        public List<DataSerie> GetDataSeries(ChartDataType type, StatField field)
        {
            IEnumerable<Sample> avgSamples = GetAvgSamples();

            //var rawSeries = from s in avgSamples
            //                group s by GetGroupingKey(s, type) into g
            //                select g;
            ////select new { g.Key, g };

            var rawSeries = from s in avgSamples
                            group s by GetGroupingKey(s, type) into g
                            select (from n in g
                                    group n by n.N into ng
                                    select GetAvg(ng));



            List<DataSerie> result = new List<DataSerie>();
            foreach (var serie in rawSeries)
            {
                DataSerie newSerie = new DataSerie()
                {
                    Name = GetSerieName(serie, type),
                    Points = serie.Select(r => new Point2D(r.N, r[field])).ToList()
                };

                result.Add(newSerie);
            }


            #region chart types
            //var chart1 = from s in avgs
            //             group s by s.Algorithm into g
            //             select g;

            //var chart2 = from s in avgs
            //             group s by s.Distribution into g
            //             select g;

            //var chart3 = from s in avgs
            //             group s by s.Sorting into g
            //             select g;

            //var chart4 = from s in avgs
            //             group s by new { s.Algorithm, s.Distribution } into g
            //             select g;

            //var chart5 = from s in avgs
            //             group s by new { s.Algorithm, s.Sorting } into g
            //             select g;

            //var chart6 = from s in avgs
            //             group s by new { s.Distribution, s.Sorting } into g
            //             select g;
            #endregion

            return result;
        }

        //public static List<Point2D> GetCoordinates(AlgorithmResult stats, StatField field)
        //{
        //    return stats.Result.Select(r => new Point2D(r.N, r[field])).ToList();
        //}
    }

    public class DataSerie
    {
        public string Name { get; set; }
        public List<Point2D> Points { get; set; }
    }
}
