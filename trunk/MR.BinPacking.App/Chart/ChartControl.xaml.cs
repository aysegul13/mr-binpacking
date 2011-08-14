﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MR.BinPacking.Library.Experiment;
using Microsoft.Win32;
using System.IO;
using MR.BinPacking.App.Utils;

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

        SolidColorBrush[] brushes = { Brushes.YellowGreen, Brushes.OrangeRed, Brushes.Purple, Brushes.RoyalBlue, Brushes.Plum, Brushes.Red };

        public ExperimentResult DataSource { get; set; }
        public List<DataSerie> DataSeries;
        public List<FunctionHandler> Functions;

        double maxY = 0.0;
        int intervalsY = 10;
        double minX = 0;
        int intervalWidthX = 1;

        ChartDataType chartDataType = ChartDataType.Algorithm;
        StatField fieldType = StatField.ExecutionTime;
        ChartType chartType = ChartType.Bars;
        bool logScale = false;

        double chartWidth, chartHeight;
        const double offsetX = 50;
        const double offsetY = 40;


        private double ConvertX(double x)
        {
            int count = DataSeries[0].Points.Count;
            if (chartType == ChartType.Bars)
                count++;

            x = (x - minX) / intervalWidthX;

            return offsetX + chartWidth * x / count;
        }

        private double ConvertY(double y)
        {
            double h;
            if (logScale)
                h = Math.Log(Math.Max(y, 1), 2) / Math.Log(Math.Max(maxY, 1), 2);
            else
                h = y / maxY;

            h = h * intervalsY / (intervalsY + 1);
            return offsetY + chartHeight * (1 - h);
        }

        private void DrawValue(double centerX, double width, double Y, Brush brush, string value)
        {
            TextBlock tblValue = new TextBlock()
            {
                Width = width,
                Background = Brushes.Transparent,
                Foreground = brush,
                FontWeight = FontWeights.Bold,
                Text = value,
                TextAlignment = TextAlignment.Center
            };
            Canvas.SetLeft(tblValue, centerX - 0.5 * width);
            Canvas.SetTop(tblValue, Y);
            Canvas.Children.Add(tblValue);
        }

        private void DrawPoint(Point2D point, Brush brush, string name)
        {
            double d = 8;

            double X1 = ConvertX(point.X);
            double Y1 = ConvertY(point.Y);

            double X2 = ConvertX(point.X + intervalWidthX);


            Ellipse ellipse = new Ellipse()
            {
                StrokeThickness = 2,
                Width = d,
                Height = d,
                Fill = brush,
                Stroke = brush,
                ToolTip = String.Format("{0} ({1}; {2})", name, point.X, point.Y)
            };

            Canvas.SetLeft(ellipse, X1 - d / 2);
            Canvas.SetTop(ellipse, Y1 - d / 2);
            Canvas.Children.Add(ellipse);

            double w = 2.0 * (X2 - X1);
            if (w >= 40.0)
                DrawValue(X1, w, Y1 - 20.0, brush, point.Y.ToString("0.##"));
        }

        private void DrawRect(Point2D point, Brush brush, int number, int count, string name)
        {
            double barWidth = 0.9 * intervalWidthX / count;
            double barOffset = 0.05 * intervalWidthX + number * barWidth;

            double X1 = ConvertX(point.X + barOffset);
            double Y1 = ConvertY(0);

            double X2 = ConvertX(point.X + barOffset + barWidth);
            double Y2 = ConvertY(point.Y);

            Rectangle rect = new Rectangle()
            {
                Width = X2 - X1,
                Height = Y1 - Y2,
                Fill = brush,
                ToolTip = String.Format("{0} ({1}; {2})", name, point.X, point.Y)
            };

            Canvas.SetLeft(rect, X1);
            Canvas.SetTop(rect, Y2);
            Canvas.Children.Add(rect);

            double w = 2.0 * (X2 - X1);
            if (w >= 40.0)
                DrawValue((X2 + X1) / 2.0, w, Y2 - 20.0, brush, point.Y.ToString("0.##"));
        }

        private void DrawFunction(FunctionHandler function)
        {
            //double max = 0.0000001;
            //foreach (var series in DataSeries)
            //    max = Math.Max(max, series.Points.Select(p => p.Y).Max());

            int count = (DataSeries[0].Points.Count - 1) * 8;
            if (chartType == ChartType.Bars)
                count += 8;

            double maxX = minX + (count - 1) * intervalWidthX / 8;

            double X = minX;
            double Y;
            if ((fieldType == StatField.ExecutionTime) || (fieldType == StatField.Result))
                Y = function.Function(X, maxX);
            else
                Y = function.Function(X, maxY);
            Y = maxY * Y;

            PathFigure myPathFigure = new PathFigure();
            myPathFigure.StartPoint = new Point(ConvertX(X), ConvertY(Y));
            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();

            for (int i = 1; i < count; i++)
            {
                X = minX + ((double)(i * intervalWidthX) / 8);

                if ((fieldType == StatField.ExecutionTime) || (fieldType == StatField.Result))
                    Y = function.Function(X, maxX);
                else
                    Y = function.Function(X, maxY);
                Y = maxY * Y;

                LineSegment myLineSegment = new LineSegment();
                myLineSegment.Point = new Point(ConvertX(X), ConvertY(Y));
                myPathSegmentCollection.Add(myLineSegment);

                if (Y > maxY)
                    break;
            }

            myPathFigure.Segments = myPathSegmentCollection;
            PathFigureCollection myPathFigureCollection = new PathFigureCollection();
            myPathFigureCollection.Add(myPathFigure);
            PathGeometry myPathGeometry = new PathGeometry();
            myPathGeometry.Figures = myPathFigureCollection;

            System.Windows.Shapes.Path myPath = new System.Windows.Shapes.Path()
            {
                Stroke = function.Color,
                ToolTip = function.Name,
                StrokeDashArray = new DoubleCollection { 6, 3 },
                StrokeThickness = 2,
                Data = myPathGeometry
            };

            Canvas.Children.Add(myPath);
        }

        private void DrawXAxis()
        {
            double Y = Canvas.ActualHeight - offsetY;

            //axis line
            Line axisX = new Line()
            {
                X1 = offsetX,
                X2 = Canvas.ActualWidth - 0.5 * offsetX,
                Y1 = Y,
                Y2 = Y,
                StrokeThickness = 2,
                Stroke = Brushes.Black
            };
            Canvas.Children.Add(axisX);

            //arrow
            axisX = new Line()
            {
                X1 = Canvas.ActualWidth - 0.5 * offsetX - 10,
                X2 = Canvas.ActualWidth - 0.5 * offsetX,
                Y1 = Y - 5,
                Y2 = Y,
                StrokeThickness = 2,
                Stroke = Brushes.Black
            };
            Canvas.Children.Add(axisX);

            axisX = new Line()
            {
                X1 = Canvas.ActualWidth - 0.5 * offsetX - 10,
                X2 = Canvas.ActualWidth - 0.5 * offsetX,
                Y1 = Y + 5,
                Y2 = Y,
                StrokeThickness = 2,
                Stroke = Brushes.Black
            };
            Canvas.Children.Add(axisX);

            TextBlock tblUnitX = new TextBlock()
            {
                Text = "N",
                FontWeight = FontWeights.Bold
            };
            Canvas.SetLeft(tblUnitX, Canvas.ActualWidth - 0.5 * offsetX + 10);
            Canvas.SetTop(tblUnitX, Y + 8);
            Canvas.Children.Add(tblUnitX);


            int count = DataSeries[0].Points.Count;
            int gap = Math.Max(count / 10 - 1, 0);
            if (chartType == ChartType.Bars)
                count++;

            int actGap = gap;
            for (int i = 0; i < count; i++)
            {
                double X = ConvertX(minX + i * intervalWidthX);
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

                if (actGap >= gap)
                {
                    //double w = ConvertX(intervalWidthX) - ConvertX(0);
                    if (chartType == ChartType.Bars)
                    {
                        double centerX = ConvertX(minX + (i + 0.5) * intervalWidthX);
                        if (i < count - 1)
                            DrawValue(centerX, 40.0, Y + 8, Brushes.Black, DataSeries[0].Points[i].X.ToString("0.##"));
                    }
                    else
                    {
                        double centerX = X;
                        DrawValue(centerX, 40.0, Y + 8, Brushes.Black, DataSeries[0].Points[i].X.ToString("0.##"));
                    }

                    actGap = 0;
                }
                else
                {
                    actGap++;
                }
            }
        }

        private void DrawYAxis()
        {
            //axis line
            Line axisY = new Line()
            {
                X1 = offsetX,
                X2 = offsetX,
                Y1 = offsetY,
                Y2 = Canvas.ActualHeight - offsetY,
                StrokeThickness = 2,
                Stroke = Brushes.Black
            };
            Canvas.Children.Add(axisY);

            //arrow
            axisY = new Line()
            {
                X1 = offsetX - 5,
                X2 = offsetX,
                Y1 = offsetY + 10,
                Y2 = offsetY,
                StrokeThickness = 2,
                Stroke = Brushes.Black
            };
            Canvas.Children.Add(axisY);

            axisY = new Line()
            {
                X1 = offsetX + 5,
                X2 = offsetX,
                Y1 = offsetY + 10,
                Y2 = offsetY,
                StrokeThickness = 2,
                Stroke = Brushes.Black
            };
            Canvas.Children.Add(axisY);

            TextBlock tblUnitY = new TextBlock()
            {
                //Width = offsetX - 8,
                Width = offsetX + 8,
                Text = StatFieldToString((StatField)cbField.SelectedValue),
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
            Canvas.SetLeft(tblUnitY, 0);
            Canvas.SetTop(tblUnitY, offsetY - 32.0);
            Canvas.Children.Add(tblUnitY);

            double h = chartHeight / (intervalsY + 1);
            for (int i = 0; i < intervalsY + 1; i++)
            {
                double Y = Canvas.ActualHeight - offsetY - i * h;
                Line newLine = new Line()
                {
                    X1 = offsetX - 5,
                    X2 = offsetX,
                    Y1 = Y,
                    Y2 = Y,
                    StrokeThickness = 2,
                    Stroke = Brushes.Black
                };
                Canvas.Children.Add(newLine);


                TextBlock tblAxisVal = new TextBlock()
                {
                    Width = offsetX - 8,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Right
                };

                if (logScale)
                    tblAxisVal.Text = Math.Pow(maxY, (double)i / intervalsY).ToString("0.##");
                else
                    tblAxisVal.Text = (i * maxY / intervalsY).ToString("0.##");

                Canvas.SetLeft(tblAxisVal, 0);
                Canvas.SetTop(tblAxisVal, Y - 10.0);
                Canvas.Children.Add(tblAxisVal);
            }
        }

        private void cbDataType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetParamsAndRefresh();
        }

        private void cbField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetParamsAndRefresh();
        }

        public void GetParamsAndRefresh()
        {
            if (cbDataType.SelectedValue != null)
                chartDataType = (ChartDataType)cbDataType.SelectedValue;

            if (cbField.SelectedValue != null)
                fieldType = (StatField)cbField.SelectedValue;

            if (DataSource != null)
            {
                DataSeries = GetDataSeries(chartDataType, fieldType);
                intervalWidthX = DataSource.Params.Step;
                //minX = DataSource.Params.MinN;
                minX = DataSource.Samples.Select(s => s.N).Min();

                AddFunctions();

                RefreshChart();
                RefreshTable();
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


        void AddChartDataTypes()
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
        }

        string StatFieldToString(StatField statField)
        {
            switch (statField)
            {
                case StatField.QualityEstimation:
                    return "oszacowanie jakości";
                case StatField.ErrorEstimation:
                    return "oszacowanie błędu";
                case StatField.Result:
                    return "wynik";
                default:    //ExecutionTime
                    return "czas działania";
            }
        }

        void AddStatFields()
        {
            //fill stat fields combo box
            cbField.Items.Add(new ComboBoxItem()
            {
                Content = StatFieldToString(StatField.ExecutionTime),
                Tag = StatField.ExecutionTime
            });

            cbField.Items.Add(new ComboBoxItem()
            {
                Content = StatFieldToString(StatField.Result),
                Tag = StatField.Result
            });

            cbField.Items.Add(new ComboBoxItem()
            {
                Content = StatFieldToString(StatField.QualityEstimation),
                Tag = StatField.QualityEstimation
            });

            cbField.Items.Add(new ComboBoxItem()
            {
                Content = StatFieldToString(StatField.ErrorEstimation),
                Tag = StatField.ErrorEstimation
            });
        }

        void AddFunctions()
        {
            Functions = new List<FunctionHandler>();

            if ((fieldType == StatField.ExecutionTime) || (fieldType == StatField.Result))
            {
                Functions.Add(new FunctionHandler()
                {
                    Name = "O(log(n))",
                    Color = Brushes.CornflowerBlue,
                    Function = FunctionHandler.FuncLogN
                });

                Functions.Add(new FunctionHandler()
                {
                    Name = "O(n)",
                    Color = Brushes.CornflowerBlue,
                    Function = FunctionHandler.FuncN
                });

                Functions.Add(new FunctionHandler()
                {
                    Name = "O(n*log(n))",
                    Color = Brushes.CornflowerBlue,
                    Function = FunctionHandler.FuncNLogN
                });

                Functions.Add(new FunctionHandler()
                {
                    Name = "O(n^2)",
                    Color = Brushes.CornflowerBlue,
                    Function = FunctionHandler.FuncNN
                });

                Functions.Add(new FunctionHandler()
                {
                    Name = "O(2^n)",
                    Color = Brushes.CornflowerBlue,
                    Function = FunctionHandler.Func2PowN
                });
            }
            else
            {
                Functions.Add(new FunctionHandler()
                {
                    Name = "11/9",
                    Color = Brushes.Red,
                    Function = FunctionHandler.Const11f9
                });

                Functions.Add(new FunctionHandler()
                {
                    Name = "17/10",
                    Color = Brushes.Red,
                    Function = FunctionHandler.Const17f10
                });

                Functions.Add(new FunctionHandler()
                {
                    Name = "2",
                    Color = Brushes.Red,
                    Function = FunctionHandler.Const2
                });
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AddChartDataTypes();
            AddStatFields();
            AddFunctions();

            cbDataType.SelectedIndex = 0;
            cbField.SelectedIndex = 0;
        }

        void DrawDataSerie(DataSerie serie, int number, int count)
        {
            for (int j = 0; j < serie.Points.Count; j++)
            {
                if (chartType != ChartType.Bars)
                {
                    if ((chartType == ChartType.Lines) && (j > 0))
                    {
                        Line newLine = new Line()
                        {
                            X1 = ConvertX(serie.Points[j - 1].X),
                            Y1 = ConvertY(serie.Points[j - 1].Y),
                            X2 = ConvertX(serie.Points[j].X),
                            Y2 = ConvertY(serie.Points[j].Y),
                            Stroke = serie.Color,
                            StrokeThickness = 2.0,
                            ToolTip = serie.Name
                        };
                        Canvas.Children.Add(newLine);
                    }

                    DrawPoint(serie.Points[j], serie.Color, serie.Name);
                }
                else
                {
                    DrawRect(serie.Points[j], serie.Color, number, count, serie.Name);
                }
            }
        }

        public void RefreshChart()
        {
            Canvas.Children.Clear();

            chartWidth = Canvas.ActualWidth - 1.5 * offsetX;
            chartHeight = Canvas.ActualHeight - 2 * offsetY;

            if ((fieldType == StatField.ErrorEstimation) || (fieldType == StatField.QualityEstimation))
                maxY = 2;
            else
                maxY = 0.01;

            IEnumerable<DataSerie> VisibleDataSeries = DataSeries.Where(ds => ds.Visible == true);
            foreach (var series in VisibleDataSeries)
                maxY = Math.Max(maxY, series.Points.Select(p => p.Y).Max());

            for (int i = 0; i < VisibleDataSeries.Count(); i++)
            {
                DataSerie serie = VisibleDataSeries.ElementAt(i);
                DrawDataSerie(serie, i, VisibleDataSeries.Count());
            }

            lbFunctions.ItemsSource = Functions;
            IEnumerable<FunctionHandler> VisibleFunctions = Functions.Where(f => f.Visible == true);
            foreach (var func in VisibleFunctions)
                DrawFunction(func);

            DrawXAxis();
            DrawYAxis();
        }

        private void RefreshTable()
        {
            gTable.Children.Clear();
            gTable.RowDefinitions.Clear();
            gTable.ColumnDefinitions.Clear();

            List<DataSerie> VisibleDataSeries = DataSeries.Where(ds => ds.Visible == true).ToList();

            for (int i = 0; i < VisibleDataSeries.Count + 1; i++)
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
                        border.BorderThickness = new Thickness(1.0);
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
                            border.BorderThickness = new Thickness(1.0);
                            textBlock.FontWeight = FontWeights.Bold;
                            textBlock.Text = VisibleDataSeries[i - 1].Name;
                        }
                        else
                        {
                            border.BorderThickness = new Thickness(0.5);
                            textBlock.Text = VisibleDataSeries[i - 1].Points[j - 1].Y.ToString();
                            textBlock.HorizontalAlignment = HorizontalAlignment.Right;
                        }
                    }

                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, j);
                    gTable.Children.Add(border);
                }
            }
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
                QualityEstimation = group.Average(s => s.QualityEstimation),
                Result = (int)Math.Ceiling(group.Average(s => s.Result)),
                ErrorEstimation = group.Average(s => s.ErrorEstimation)
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
            RefreshTable();
        }

        private void bSaveImg_Click(object sender, RoutedEventArgs e)
        {
            Loader.SaveToImg(Canvas, Canvas.ActualWidth, Canvas.ActualHeight);
        }


        private double prevW = -10.0;
        private double prevH = -10.0;
        private void svScroll_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((e.WidthChanged) || (e.HeightChanged))
            {
                double maxChange = Math.Max(Math.Abs(e.NewSize.Width - prevW), Math.Abs(e.NewSize.Height - prevH));
                if (maxChange > 4.0)
                {
                    prevW = e.NewSize.Width;
                    prevH = e.NewSize.Height;
                    //GetParamsAndRefresh();
                    RefreshChart();
                }
            }
        }

        private void FuncCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            RefreshChart();
        }

        private void bSaveTable_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();
            if (saveDialog.ShowDialog() == true)
                SaveResultsToFile(saveDialog.FileName);
        }

        private string GetTableName()
        {
            string name = "";
            switch (chartDataType)
            {
                case ChartDataType.Distribution:
                    name = "PORÓWNANIE ROZKŁADÓW";
                    break;
                case ChartDataType.Sorting:
                    name = "PORÓWNANIE SORTOWANIA";
                    break;
                case ChartDataType.AlgorithmDistribution:
                    name = "PORÓWNANIE ALGORYTMÓW I ROZKŁADÓW";
                    break;
                case ChartDataType.AlgorithmSorting:
                    name = "PORÓWNANIE ALGORYTMÓW I SORTOWANIA";
                    break;
                case ChartDataType.DistributionSorting:
                    name = "PORÓWNANIE ROZKŁADÓW I SORTOWANIA";
                    break;
                default:
                    name = "PORÓWNANIE ALGORYTMÓW";
                    break;
            }

            name += " - ";

            switch (fieldType)
            {
                case StatField.QualityEstimation:
                    return name + "dolne ograniczenie [liczba elementów]";
                case StatField.ErrorEstimation:
                    return name + "silniejsze dolne ograniczenie [liczba elementów]";
                case StatField.Result:
                    return name + "wynik [liczba elementów]";
                default:
                    return name + "czas działania [ms]";
            }
        }

        public void SaveResultsToFile(string filename)
        {
            string separator = ";";

            using (StreamWriter sw = new StreamWriter(filename))
            {
                sw.WriteLine(GetTableName() + ";");

                List<DataSerie> VisibleDataSeries = DataSeries.Where(ds => ds.Visible == true).ToList();
                for (int j = 0; j < DataSeries[0].Points.Count + 1; j++)
                {
                    for (int i = 0; i < VisibleDataSeries.Count + 1; i++)
                    {
                        if (i == 0)
                        {
                            if (j == 0)
                            {
                                sw.Write(@"N \ Seria danych" + separator);
                            }
                            else
                            {
                                int N = DataSource.Params.MinN + (j - 1) * DataSource.Params.Step;
                                sw.Write(N.ToString() + separator);
                            }
                        }
                        else
                        {
                            if (j == 0)
                                sw.Write(VisibleDataSeries[i - 1].Name + separator);
                            else
                                sw.Write(VisibleDataSeries[i - 1].Points[j - 1].Y.ToString() + separator);
                        }
                    }

                    sw.WriteLine();
                }
            }
        }

        private void bSaveTableImg_Click(object sender, RoutedEventArgs e)
        {
            Loader.SaveToImg(gTable, gTable.ActualWidth, gTable.ActualHeight);
        }
    }
}