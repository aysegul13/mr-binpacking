using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MR.BinPacking.App.Controls
{
    internal enum PredefinedColorPresentationMode { Darker, Lighter, Mixed }
    public enum ColorPresenterType { Default, Top, Middle, Bottom }

    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        private Color color;
        public Color Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;

                colorSample.Fill = new SolidColorBrush(color);
                colorLabel.Content = color.ToString();
                currentColorPresenter.SetColor(color);

                if (this.ColorChanged != null)
                    this.ColorChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler ColorChanged;

        public ColorPicker()
        {
            InitializeComponent();

            AddPredefinedColor(Colors.White, "Biały", PredefinedColorPresentationMode.Darker);
            AddPredefinedColor(Colors.Black, "Czarny", PredefinedColorPresentationMode.Lighter);
            AddPredefinedColor(Colors.Red, "Czerwony", PredefinedColorPresentationMode.Mixed);
            AddPredefinedColor(Colors.Orange, "Pomarańczowy", PredefinedColorPresentationMode.Mixed);
            AddPredefinedColor(Colors.Yellow, "Żółty", PredefinedColorPresentationMode.Mixed);
            AddPredefinedColor(Colors.Green, "Zielony", PredefinedColorPresentationMode.Mixed);
            AddPredefinedColor(Colors.Blue, "Niebieski", PredefinedColorPresentationMode.Mixed);
            AddPredefinedColor(Colors.Purple, "Fioletowy", PredefinedColorPresentationMode.Mixed);
        }

        #region Methods

        private void AddPredefinedColor(Color predefinedColor, string colorName, PredefinedColorPresentationMode predefinedColorPresentationMode)
        {
            StackPanel predefinedColorStackPanel = new StackPanel();
            predefinedColorStackPanel.Orientation = Orientation.Vertical;
            predefinedColorStackPanel.Margin = new Thickness(0.0, 2.0, 0.0, 2.0);
            predefinedColorStackPanel.Width = 20.0;

            ColorPresenter predefinedColorPresenter = new ColorPresenter(predefinedColor);
            predefinedColorPresenter.Margin = new Thickness(0.0, 0.0, 0.0, 4.0);
            predefinedColorPresenter.ToolTip = colorName;
            predefinedColorPresenter.MouseDown += new MouseButtonEventHandler(colorPresenter_MouseDown);
            predefinedColorStackPanel.Children.Add(predefinedColorPresenter);

            ColorPresenter[] colorPresenters = new ColorPresenter[5];

            switch (predefinedColorPresentationMode)
            {
                case PredefinedColorPresentationMode.Darker:
                    colorPresenters[0] = new ColorPresenter(AdjustColorBrightness(predefinedColor, -0.05), ColorPresenterType.Top);
                    colorPresenters[0].ToolTip = colorName + ", ciemniejszy o 5%";

                    colorPresenters[1] = new ColorPresenter(AdjustColorBrightness(predefinedColor, -0.15), ColorPresenterType.Middle);
                    colorPresenters[1].ToolTip = colorName + ", ciemniejszy o 15%";

                    colorPresenters[2] = new ColorPresenter(AdjustColorBrightness(predefinedColor, -0.25), ColorPresenterType.Middle);
                    colorPresenters[2].ToolTip = colorName + ", ciemniejszy o 25%";

                    colorPresenters[3] = new ColorPresenter(AdjustColorBrightness(predefinedColor, -0.35), ColorPresenterType.Middle);
                    colorPresenters[3].ToolTip = colorName + ", ciemniejszy o 35%";

                    colorPresenters[4] = new ColorPresenter(AdjustColorBrightness(predefinedColor, -0.5), ColorPresenterType.Bottom);
                    colorPresenters[4].ToolTip = colorName + ", ciemniejszy o 50%";
                    break;
                case PredefinedColorPresentationMode.Lighter:
                    colorPresenters[0] = new ColorPresenter(AdjustColorBrightness(predefinedColor, 0.5), ColorPresenterType.Top);
                    colorPresenters[0].ToolTip = colorName + ", jaśniejszy o 50%";

                    colorPresenters[1] = new ColorPresenter(AdjustColorBrightness(predefinedColor, 0.35), ColorPresenterType.Middle);
                    colorPresenters[1].ToolTip = colorName + ", jaśniejszy o 35%";

                    colorPresenters[2] = new ColorPresenter(AdjustColorBrightness(predefinedColor, 0.25), ColorPresenterType.Middle);
                    colorPresenters[2].ToolTip = colorName + ", jaśniejszy o 25%";

                    colorPresenters[3] = new ColorPresenter(AdjustColorBrightness(predefinedColor, 0.15), ColorPresenterType.Middle);
                    colorPresenters[3].ToolTip = colorName + ", jaśniejszy o 15%";

                    colorPresenters[4] = new ColorPresenter(AdjustColorBrightness(predefinedColor, 0.05), ColorPresenterType.Bottom);
                    colorPresenters[4].ToolTip = colorName + ", jaśniejszy o 5%";
                    break;
                case PredefinedColorPresentationMode.Mixed:
                    colorPresenters[0] = new ColorPresenter(AdjustColorBrightness(predefinedColor, 0.8), ColorPresenterType.Top);
                    colorPresenters[0].ToolTip = colorName + ", jaśniejszy o 80%";

                    colorPresenters[1] = new ColorPresenter(AdjustColorBrightness(predefinedColor, 0.6), ColorPresenterType.Middle);
                    colorPresenters[1].ToolTip = colorName + ", jaśniejszy o 60%";

                    colorPresenters[2] = new ColorPresenter(AdjustColorBrightness(predefinedColor, 0.4), ColorPresenterType.Middle);
                    colorPresenters[2].ToolTip = colorName + ", jaśniejszy o 40%";

                    colorPresenters[3] = new ColorPresenter(AdjustColorBrightness(predefinedColor, -0.25), ColorPresenterType.Middle);
                    colorPresenters[3].ToolTip = colorName + ", ciemniejszy o 25%";

                    colorPresenters[4] = new ColorPresenter(AdjustColorBrightness(predefinedColor, -0.5), ColorPresenterType.Bottom);
                    colorPresenters[4].ToolTip = colorName + ", ciemniejszy o 50%";
                    break;
            }

            colorPresenters[0].MouseDown += new MouseButtonEventHandler(colorPresenter_MouseDown);
            predefinedColorStackPanel.Children.Add(colorPresenters[0]);

            colorPresenters[1].MouseDown += new MouseButtonEventHandler(colorPresenter_MouseDown);
            predefinedColorStackPanel.Children.Add(colorPresenters[1]);

            colorPresenters[2].MouseDown += new MouseButtonEventHandler(colorPresenter_MouseDown);
            predefinedColorStackPanel.Children.Add(colorPresenters[2]);

            colorPresenters[3].MouseDown += new MouseButtonEventHandler(colorPresenter_MouseDown);
            predefinedColorStackPanel.Children.Add(colorPresenters[3]);

            colorPresenters[4].MouseDown += new MouseButtonEventHandler(colorPresenter_MouseDown);
            predefinedColorStackPanel.Children.Add(colorPresenters[4]);

            colorsStackPanel.Children.Add(predefinedColorStackPanel);
        }

        private Color AdjustColorBrightness(Color colorToAdjust, double adjustment)
        {
            Color adjustedColor = new Color();

            adjustedColor.A = colorToAdjust.A;

            if (adjustment > 0)
            {
                adjustedColor.R = (Byte)Math.Min(255, Math.Floor(colorToAdjust.R + 255 * adjustment));
                adjustedColor.G = (Byte)Math.Min(255, Math.Floor(colorToAdjust.G + 255 * adjustment));
                adjustedColor.B = (Byte)Math.Min(255, Math.Floor(colorToAdjust.B + 255 * adjustment));
            }
            else
            {
                adjustedColor.R = (Byte)Math.Max(0, Math.Floor(colorToAdjust.R + 255 * adjustment));
                adjustedColor.G = (Byte)Math.Max(0, Math.Floor(colorToAdjust.G + 255 * adjustment));
                adjustedColor.B = (Byte)Math.Max(0, Math.Floor(colorToAdjust.B + 255 * adjustment));
            }

            return adjustedColor;
        }

        #endregion

        #region Events

        private void expandButton_Click(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = true;
        }

        private void colorPresenter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Color = (sender as ColorPresenter).GetColor();

            popup.IsOpen = false;
        }

        #endregion
    }

    public class ColorPresenter : Canvas
    {
        private Border innerBorder = null;
        private Border outerBorder = null;

        private ColorPresenterType colorPresenterType;

        public ColorPresenter() : this(Colors.Transparent, ColorPresenterType.Default) { }

        public ColorPresenter(Color color) : this(color, ColorPresenterType.Default) { }

        public ColorPresenter(Color color, ColorPresenterType colorPresenterType)
        {
            this.Width = 14.0;
            this.Height = 14.0;

            this.Background = new SolidColorBrush(color);

            this.colorPresenterType = colorPresenterType;

            innerBorder = new Border();
            innerBorder.Width = 14.0;
            innerBorder.Height = 14.0;

            switch (colorPresenterType)
            {
                case ColorPresenterType.Default:
                    innerBorder.BorderThickness = new Thickness(1.0);
                    break;
                case ColorPresenterType.Top:
                    innerBorder.BorderThickness = new Thickness(1.0, 1.0, 1.0, 0.0);
                    break;
                case ColorPresenterType.Middle:
                    innerBorder.BorderThickness = new Thickness(1.0, 0.0, 1.0, 0.0);
                    break;
                case ColorPresenterType.Bottom:
                    innerBorder.BorderThickness = new Thickness(1.0, 0.0, 1.0, 1.0);
                    break;
            }

            innerBorder.BorderBrush = Brushes.Gray;

            this.Children.Add(innerBorder);

            outerBorder = new Border();
            outerBorder.Width = 16.0;
            outerBorder.Height = 16.0;
            outerBorder.BorderThickness = new Thickness(1.0);
            outerBorder.BorderBrush = Brushes.Orange;
            outerBorder.Visibility = Visibility.Collapsed;

            Canvas.SetTop(outerBorder, -1.0);
            Canvas.SetLeft(outerBorder, -1.0);

            this.Children.Add(outerBorder);

            Canvas.SetZIndex(this, 0);

            this.MouseEnter += new MouseEventHandler(ColorPresenter_MouseEnter);
            this.MouseLeave += new MouseEventHandler(ColorPresenter_MouseLeave);
        }

        #region Methods

        public Color GetColor()
        {
            return (this.Background as SolidColorBrush).Color;
        }

        public void SetColor(Color color)
        {
            this.Background = new SolidColorBrush(color);
        }

        #endregion

        #region Events

        private void ColorPresenter_MouseEnter(object sender, MouseEventArgs e)
        {
            innerBorder.BorderThickness = new Thickness(1.0);

            innerBorder.BorderBrush = Brushes.White;

            outerBorder.Visibility = Visibility.Visible;

            Canvas.SetZIndex(this, 1);
        }

        private void ColorPresenter_MouseLeave(object sender, MouseEventArgs e)
        {
            switch (colorPresenterType)
            {
                case ColorPresenterType.Default:
                    innerBorder.BorderThickness = new Thickness(1.0);
                    break;
                case ColorPresenterType.Top:
                    innerBorder.BorderThickness = new Thickness(1.0, 1.0, 1.0, 0.0);
                    break;
                case ColorPresenterType.Middle:
                    innerBorder.BorderThickness = new Thickness(1.0, 0.0, 1.0, 0.0);
                    break;
                case ColorPresenterType.Bottom:
                    innerBorder.BorderThickness = new Thickness(1.0, 0.0, 1.0, 1.0);
                    break;
            }

            innerBorder.BorderBrush = Brushes.Gray;

            outerBorder.Visibility = Visibility.Collapsed;

            Canvas.SetZIndex(this, 0);
        }

        #endregion
    }
}
