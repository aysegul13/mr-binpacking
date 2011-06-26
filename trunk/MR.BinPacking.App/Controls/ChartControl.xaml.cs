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

namespace MR.BinPacking.App.Controls
{
    /// <summary>
    /// Interaction logic for ChartControl.xaml
    /// </summary>
    public partial class ChartControl : UserControl
    {
        public ChartControl()
        {
            InitializeComponent();
        }

        public List<List<Point2D>> DataSeries;
        double AxisYMax = 0.0;
        int AxisYIntervals = 10;

        public double AxisXMin = 0;
        public int AxisXIntervalWidth = 1;

        bool logScale = false;
        bool barChart = true;
        bool drawLine = true;
        bool showLabels = true;


        double chartWidth, chartHeight;
        const double chartOffsetX = 40;
        const double chartOffsetY = 40;


        public double ConvertX(double x)
        {
            int count = DataSeries[0].Count;
            if (barChart)
                count++;

            x = (x - AxisXMin) / AxisXIntervalWidth;

            return chartOffsetX + chartWidth * x / count;
        }

        public double ConvertY(double y)
        {
            double h;
            if (logScale)
                h = Math.Log(Math.Max(y, 1), 2) / AxisYMax;
            else
                h = y / AxisYMax;

            h = h * AxisYIntervals / (AxisYIntervals + 1);
            return chartOffsetY + chartHeight * (1 - h);
        }

        public void DrawRect(double x, double y, Brush brush, int number, int count)
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
                ToolTip = String.Format("({0}; {1})", x, y)
            };

            Canvas.SetLeft(rect, X1);
            Canvas.SetTop(rect, Y2);

            Canvas.Children.Add(rect);
        }

        public void DrawPoint(double x, double y, Brush brush)
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
                ToolTip = String.Format("({0}; {1})", x, y)
            };

            Canvas.SetLeft(ellipse, newX - d / 2);
            Canvas.SetTop(ellipse, newY - d / 2);
            Canvas.Children.Add(ellipse);
        }

        public void Refresh()
        {
            Canvas.Children.Clear();
            AxisYMax = 0.01;

            foreach (var series in DataSeries)
            {
                if (logScale)
                    AxisYMax = Math.Max(AxisYMax, series.Select(p => Math.Log(Math.Max(p.Y, 1), 2)).Max());
                else
                    AxisYMax = Math.Max(AxisYMax, series.Select(p => p.Y).Max());
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

            for (int i = 0; i < DataSeries.Count; i++)
            {
                Brush brush;
                switch (i)
                {
                    case 0:
                        brush = Brushes.YellowGreen;
                        break;
                    case 1:
                        brush = Brushes.Purple;
                        break;
                    case 2:
                        brush = Brushes.OrangeRed;
                        break;
                    default:
                        brush = Brushes.RoyalBlue;
                        break;
                }

                for (int j = 0; j < DataSeries[i].Count; j++)
                {
                    if (!barChart)
                    {
                        if (drawLine && (j > 0))
                        {
                            Line newLine = new Line()
                            {
                                X1 = ConvertX(DataSeries[i][j - 1].X),
                                X2 = ConvertX(DataSeries[i][j].X),
                                Y1 = ConvertY(DataSeries[i][j - 1].Y),
                                Y2 = ConvertY(DataSeries[i][j].Y),
                                Stroke = brush,
                                StrokeThickness = 2.0
                            };
                            Canvas.Children.Add(newLine);
                        }

                        DrawPoint(DataSeries[i][j].X, DataSeries[i][j].Y, brush);
                    }
                    else
                    {
                        DrawRect(DataSeries[i][j].X, DataSeries[i][j].Y, brush, i, DataSeries.Count);
                    }
                }
            }

            DrawFunction();

            DrawXAxis();
            DrawYAxis();
        }

        public void DrawFunction()
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
                max = Math.Max(max, series.Select(p => p.Y).Max());

            int count = DataSeries[0].Count * 4;
            if (barChart)
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


        public void DrawXAxis()
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


            int count = DataSeries[0].Count;
            if (barChart)
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

        public void DrawYAxis()
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

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ExperimentParams prms = new ExperimentParams()
            {
                Algorithms = new List<ListAlgorithm>(),
                BinSize = 100,
                Dist = Distribution.Uniform,
                MinN = 100,
                MaxN = 1000,
                Step = 100,
                MinVal = 0.0,
                MaxVal = 1.0,
                Repeat = 2
            };

            prms.Algorithms.Add(new BestFitDecreasing() { IsPresentation = false });
            prms.Algorithms.Add(new BestFit() { IsPresentation = false });


            DataSeries = new List<List<Point2D>>();

            ExperimentResult stats = Experiment.ExecuteExperiment(prms);
            for (int i = 0; i < stats.DataSeries.Count; i++)
                DataSeries.Add(Experiment.GetCoordinates(stats[i], StatField.ExecutionTime));


            AxisXIntervalWidth = prms.Step;
            AxisXMin = prms.MinN;

            //DataSeries = new List<Point2D>();
            //for (int i = 0; i < 10; i++)
            //{
            //    DataSeries.Add(new Point2D(i + 1, (i + 1) * 10));
            //    //Points.Add(new Point2D(i + 1, Math.Pow(2, i)));
            //}
        }

        private void tgbType_Click(object sender, RoutedEventArgs e)
        {
            barChart = !barChart;
            Refresh();
        }

        private void tgbScale_Click(object sender, RoutedEventArgs e)
        {
            logScale = !logScale;
            Refresh();
        }

        private void tgbLines_Click(object sender, RoutedEventArgs e)
        {
            drawLine = !drawLine;
            Refresh();
        }
    }
}
