using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MR.BinPacking.Library;
using MR.BinPacking.Library.Base;
using MR.BinPacking.Library.Experiment;
using MR.BinPacking.Library.Utils;
using MR.BinPacking.Library.Algorithms;

namespace MR.BinPacking.App.Chart
{
    public enum ChartType { Bars, Lines, Points };

    /// <summary>
    /// Interaction logic for ChartControl.xaml
    /// </summary>
    public partial class ChartControl : UserControl
    {
        public ChartControl()
        {
            InitializeComponent();
        }

        private SolidColorBrush[] brushes = { Brushes.YellowGreen, Brushes.OrangeRed, Brushes.Purple, Brushes.RoyalBlue, Brushes.Plum, Brushes.Red };

        public ExperimentResult DataSource { get; set; }
        public List<DataSerie> DataSeries;

        double AxisYMax = 0.0;
        int AxisYIntervals = 10;
        public double AxisXMin = 0;
        public int AxisXIntervalWidth = 1;

        ChartDataType chartDataType = ChartDataType.Algorithm;
        StatField fieldType = StatField.ExecutionTime;
        ChartType chartType = ChartType.Bars;
        bool logScale = false;

        double chartWidth, chartHeight;
        const double chartOffsetX = 40;
        const double chartOffsetY = 40;


        private double ConvertX(double x)
        {
            int count = DataSeries[0].Points.Count;
            if (chartType == ChartType.Bars)
                count++;

            x = (x - AxisXMin) / AxisXIntervalWidth;

            return chartOffsetX + chartWidth * x / count;
        }

        private double ConvertY(double y)
        {
            double h;
            if (logScale)
                h = Math.Log(Math.Max(y, 1), 2) / AxisYMax;
            else
                h = y / AxisYMax;

            h = h * AxisYIntervals / (AxisYIntervals + 1);
            return chartOffsetY + chartHeight * (1 - h);
        }

        private void DrawRect(double x, double y, Brush brush, int number, int count, string name)
        {
            double barWidth = 0.9 * AxisXIntervalWidth / count;
            double barOffset = 0.05 * AxisXIntervalWidth + number * barWidth;

            double X1 = ConvertX(x + barOffset);
            double X2 = ConvertX(x + barOffset + barWidth);

            //double X1 = ConvertX(x + 0.1 * AxisXIntervalWidth);
            //double X2 = ConvertX(x + 0.9 * AxisXIntervalWidth);
            double Y1 = ConvertY(0);
            double Y2 = ConvertY(y);

            Rectangle rect = new Rectangle()
            {
                Width = X2 - X1,
                Height = Y1 - Y2,
                Fill = brush,
                ToolTip = String.Format("{0} ({1}; {2})", name, x, y)
            };

            Canvas.SetLeft(rect, X1);
            Canvas.SetTop(rect, Y2);

            Canvas.Children.Add(rect);
        }

        private void DrawPoint(double x, double y, Brush brush, string name)
        {
            double newX = ConvertX(x);
            double newY = ConvertY(y);
            double d = 6;

            Ellipse ellipse = new Ellipse()
            {
                StrokeThickness = 2,
                Width = d,
                Height = d,
                Stroke = brush,
                ToolTip = String.Format("{0} ({1}; {2})", name, x, y)
            };

            Canvas.SetLeft(ellipse, newX - d / 2);
            Canvas.SetTop(ellipse, newY - d / 2);
            Canvas.Children.Add(ellipse);
        }

        private void DrawFunction()
        {
            double X = AxisXMin;
            //double Y = Math.Log(X, 2);
            //double Y = X * X;
            //double Y = Math.Pow(2, X);
            double Y = X;

            PathFigure myPathFigure = new PathFigure();
            myPathFigure.StartPoint = new Point(ConvertX(X), ConvertY(Y));
            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();

            double max = 0.01;
            foreach (var series in DataSeries)
                max = Math.Max(max, series.Points.Select(p => p.Y).Max());

            int count = DataSeries[0].Points.Count * 4;
            if (chartType == ChartType.Bars)
                count += 4;

            for (int i = 1; i < count; i++)
            {
                X = AxisXMin + i * AxisXIntervalWidth;
                //Y = Math.Log(X, 2);
                //Y = X * X;
                //Y = Math.Pow(2, X);
                Y = X;

                LineSegment myLineSegment = new LineSegment();
                myLineSegment.Point = new Point(ConvertX(X), ConvertY(Y));
                myPathSegmentCollection.Add(myLineSegment);

                if (Y > max)
                    break;
            }

            myPathFigure.Segments = myPathSegmentCollection;
            PathFigureCollection myPathFigureCollection = new PathFigureCollection();
            myPathFigureCollection.Add(myPathFigure);
            PathGeometry myPathGeometry = new PathGeometry();
            myPathGeometry.Figures = myPathFigureCollection;

            Path myPath = new Path();
            myPath.Stroke = Brushes.CornflowerBlue;
            myPath.StrokeThickness = 2;
            myPath.Data = myPathGeometry;

            Canvas.Children.Add(myPath);
        }


        private void DrawXAxis()
        {
            double Y = Canvas.ActualHeight - chartOffsetY;

            //axis line
            Line axisX = new Line()
            {
                X1 = chartOffsetX,
                X2 = Canvas.ActualWidth - chartOffsetX,
                Y1 = Y,
                Y2 = Y,
                StrokeThickness = 2,
                Stroke = Brushes.Black
            };
            Canvas.Children.Add(axisX);

            //arrow
            axisX = new Line()
            {
                X1 = Canvas.ActualWidth - chartOffsetX - 10,
                X2 = Canvas.ActualWidth - chartOffsetX,
                Y1 = Y - 5,
                Y2 = Y,
                StrokeThickness = 2,
                Stroke = Brushes.Black
            };
            Canvas.Children.Add(axisX);

            axisX = new Line()
            {
                X1 = Canvas.ActualWidth - chartOffsetX - 10,
                X2 = Canvas.ActualWidth - chartOffsetX,
                Y1 = Y + 5,
                Y2 = Y,
                StrokeThickness = 2,
                Stroke = Brushes.Black
            };
            Canvas.Children.Add(axisX);


            int count = DataSeries[0].Points.Count;
            if (chartType == ChartType.Bars)
                count++;

            for (int i = 0; i < count; i++)
            {
                double X = ConvertX(AxisXMin + i * AxisXIntervalWidth);
                Line newLine = new Line()
                {
                    X1 = X,
                    X2 = X,
                    Y1 = Y,
                    Y2 = Y + 5,
                    StrokeThickness = 2,
                    Stroke = Brushes.Black
                };
                Canvas.Children.Add(newLine);
            }
        }

        private void DrawYAxis()
        {
            //axis line
            Line axisY = new Line()
            {
                X1 = chartOffsetX,
                X2 = chartOffsetX,
                Y1 = chartOffsetY,
                Y2 = Canvas.ActualHeight - chartOffsetY,
                StrokeThickness = 2,
                Stroke = Brushes.Black
            };
            Canvas.Children.Add(axisY);

            //arrow
            axisY = new Line()
            {
                X1 = chartOffsetX - 5,
                X2 = chartOffsetX,
                Y1 = chartOffsetY + 10,
                Y2 = chartOffsetY,
                StrokeThickness = 2,
                Stroke = Brushes.Black
            };
            Canvas.Children.Add(axisY);

            axisY = new Line()
            {
                X1 = chartOffsetX + 5,
                X2 = chartOffsetX,
                Y1 = chartOffsetY + 10,
                Y2 = chartOffsetY,
                StrokeThickness = 2,
                Stroke = Brushes.Black
            };
            Canvas.Children.Add(axisY);


            double h = chartHeight / (AxisYIntervals + 1);
            for (int i = 0; i < AxisYIntervals + 1; i++)
            {
                double Y = Canvas.ActualHeight - chartOffsetY - i * h;
                Line newLine = new Line()
                {
                    X1 = chartOffsetX - 5,
                    X2 = chartOffsetX,
                    Y1 = Y,
                    Y2 = Y,
                    StrokeThickness = 2,
                    Stroke = Brushes.Black
                };
                Canvas.Children.Add(newLine);

                Label newLabel = new Label()
                {
                    Width = chartOffsetX - 5,
                    FlowDirection = FlowDirection.RightToLeft,
                    Content = (i * AxisYMax / AxisYIntervals).ToString("0.##"),
                    Padding = new Thickness(0.0)
                };
                Canvas.SetLeft(newLabel, 0);
                Canvas.SetTop(newLabel, Y);
                Canvas.Children.Add(newLabel);
            }
        }

        private void cbDataType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            chartDataType = (ChartDataType)cbDataType.SelectedValue;

            if (DataSource != null)
            {
                DataSeries = GetDataSeries(chartDataType, fieldType);
                AxisXIntervalWidth = DataSource.Params.Step;
                AxisXMin = DataSource.Params.MinN;

                RefreshChart();
                lbLegend.ItemsSource = DataSeries;
            }
        }

        private void cbField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            fieldType = (StatField)cbField.SelectedValue;

            if (DataSource != null)
            {
                DataSeries = GetDataSeries(chartDataType, fieldType);
                AxisXIntervalWidth = DataSource.Params.Step;
                AxisXMin = DataSource.Params.MinN;

                RefreshChart();
                lbLegend.ItemsSource = DataSeries;
            }
        }

        private void bType_Click(object sender, RoutedEventArgs e)
        {
            switch (chartType)
            {
                case ChartType.Bars:
                    chartType = ChartType.Lines;
                    bType.Content = "liniowy";
                    break;
                case ChartType.Lines:
                    chartType = ChartType.Points;
                    bType.Content = "punktowy";
                    break;
                case ChartType.Points:
                    chartType = ChartType.Bars;
                    bType.Content = "słupkowy";
                    break;
            }

            RefreshChart();
        }

        private void bScale_Click(object sender, RoutedEventArgs e)
        {
            logScale = !logScale;

            if (logScale)
                bScale.Content = "logarytmiczna";
            else
                bScale.Content = "liniowa";

            RefreshChart();
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //fill chart data types combo box
            ComboBoxItem item = new ComboBoxItem() { Content = "Algorytm", Tag = ChartDataType.Algorithm };
            cbDataType.Items.Add(item);

            item = new ComboBoxItem() { Content = "Rozkład", Tag = ChartDataType.Distribution };
            cbDataType.Items.Add(item);

            item = new ComboBoxItem() { Content = "Sortowanie", Tag = ChartDataType.Sorting };
            cbDataType.Items.Add(item);

            item = new ComboBoxItem() { Content = "Algorytm/rozkład", Tag = ChartDataType.AlgorithmDistribution };
            cbDataType.Items.Add(item);

            item = new ComboBoxItem() { Content = "Algorytm/sortowanie", Tag = ChartDataType.AlgorithmSorting };
            cbDataType.Items.Add(item);

            item = new ComboBoxItem() { Content = "Rozkład/sortowanie", Tag = ChartDataType.DistributionSorting };
            cbDataType.Items.Add(item);

            cbDataType.SelectedIndex = 0;

            //fill stat fields combo box
            item = new ComboBoxItem() { Content = "czas działania", Tag = StatField.ExecutionTime };
            cbField.Items.Add(item);

            item = new ComboBoxItem() { Content = "wynik", Tag = StatField.Result };
            cbField.Items.Add(item);

            item = new ComboBoxItem() { Content = "dolne ograniczenie", Tag = StatField.LowerBound };
            cbField.Items.Add(item);

            item = new ComboBoxItem() { Content = "silniejsze dolne ograniczenie", Tag = StatField.StrongerLowerBound };
            cbField.Items.Add(item);

            cbField.SelectedIndex = 0;
        }

        public void RefreshChart()
        {
            Canvas.Children.Clear();
            AxisYMax = 0.01;

            IEnumerable<DataSerie> VisibleDS = DataSeries.Where(ds => ds.Visible == true);
            foreach (var series in VisibleDS)
            {
                if (logScale)
                    AxisYMax = Math.Max(AxisYMax, series.Points.Select(p => Math.Log(Math.Max(p.Y, 1), 2)).Max());
                else
                    AxisYMax = Math.Max(AxisYMax, series.Points.Select(p => p.Y).Max());
            }

            //int factor = 1;
            //while (AxisYMax > 1.0)
            //{
            //    AxisYMax /= 10.0;
            //    factor *= 10;
            //}
            //AxisYMax = factor;

            chartWidth = Canvas.ActualWidth - 2 * chartOffsetX;
            chartHeight = Canvas.ActualHeight - 2 * chartOffsetY;

            for (int i = 0; i < VisibleDS.Count(); i++)
            {
                DataSerie serie = VisibleDS.ElementAt(i);
                for (int j = 0; j < serie.Points.Count; j++)
                {
                    if (chartType != ChartType.Bars)
                    {
                        if ((chartType == ChartType.Lines) && (j > 0))
                        {
                            Line newLine = new Line()
                            {
                                X1 = ConvertX(serie.Points[j - 1].X),
                                X2 = ConvertX(serie.Points[j].X),
                                Y1 = ConvertY(serie.Points[j - 1].Y),
                                Y2 = ConvertY(serie.Points[j].Y),
                                Stroke = serie.Color,
                                StrokeThickness = 2.0,
                                ToolTip = serie.Name
                            };
                            Canvas.Children.Add(newLine);
                        }

                        DrawPoint(serie.Points[j].X, serie.Points[j].Y, serie.Color, serie.Name);
                    }
                    else
                    {
                        DrawRect(serie.Points[j].X, serie.Points[j].Y, serie.Color, i, VisibleDS.Count(), serie.Name);
                    }
                }
            }

            DrawFunction();

            DrawXAxis();
            DrawYAxis();
        }

        private void RefreshTable()
        {
            gTable.Children.Clear();
            gTable.RowDefinitions.Clear();
            gTable.ColumnDefinitions.Clear();

            for (int i = 0; i < DataSeries.Count + 1; i++)
            {
                gTable.RowDefinitions.Add(new RowDefinition());

                for (int j = 0; j < DataSeries[0].Points.Count + 1; j++)
                {
                    Border border = new Border()
                    {
                        BorderBrush = Brushes.Black
                    };

                    TextBlock textBlock = new TextBlock();
                    textBlock.Margin = new Thickness(2.0);
                    border.Child = textBlock;

                    if (i == 0)
                    {
                        gTable.ColumnDefinitions.Add(new ColumnDefinition() { MaxWidth = 100.0 });
                        border.BorderThickness = new Thickness(2.0);
                        textBlock.FontWeight = FontWeights.Bold;

                        if (j == 0)
                        {
                            gTable.ColumnDefinitions.Last().Width = new GridLength(1.0, GridUnitType.Auto);
                            textBlock.Text = "Seria danych \\ N";
                        }
                        else
                        {
                            int N = DataSource.Params.MinN + (j - 1) * DataSource.Params.Step;
                            textBlock.Text = N.ToString();
                            textBlock.HorizontalAlignment = HorizontalAlignment.Right;
                        }
                    }
                    else
                    {
                        if (j == 0)
                        {
                            border.BorderThickness = new Thickness(2.0);
                            textBlock.FontWeight = FontWeights.Bold;
                            textBlock.Text = DataSeries[i - 1].Name;
                        }
                        else
                        {
                            border.BorderThickness = new Thickness(1.0);
                            textBlock.Text = DataSeries[i - 1].Points[j - 1].Y.ToString();
                            textBlock.HorizontalAlignment = HorizontalAlignment.Right;
                        }
                    }

                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);
                    gTable.Children.Add(border);
                }
            }

            RefreshChart();
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            RefreshTable();
        }

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

        private IEnumerable<Sample> GetAvgSamples()
        {
            return from s in DataSource.Samples
                   group s by s.ID % (DataSource.Samples.Count / DataSource.Params.Repeat) into g
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

        private List<DataSerie> GetDataSeries(ChartDataType type, StatField field)
        {
            IEnumerable<Sample> avgSamples = GetAvgSamples();

            var rawSeries = from s in avgSamples
                            group s by GetGroupingKey(s, type) into g
                            select (from n in g
                                    group n by n.N into ng
                                    select GetAvg(ng));



            List<DataSerie> result = new List<DataSerie>();
            for (int i = 0; i < rawSeries.Count(); i++)
            {
                IEnumerable<Sample> serie = rawSeries.ElementAt(i);
                DataSerie newSerie = new DataSerie()
                {
                    Color = brushes[i % brushes.Length],
                    Name = GetSerieName(serie, type),
                    Points = serie.Select(r => new Point2D(r.N, r[field])).ToList()
                };

                result.Add(newSerie);
            }

            return result;
        }

        private void CheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            RefreshChart();
        }
    }
}
