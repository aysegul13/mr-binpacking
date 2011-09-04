﻿using System;
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
using System.Collections.ObjectModel;
using MR.BinPacking.Library.Base;
using System.Windows.Media.Animation;

namespace MR.BinPacking.App.Controls
{
    /// <summary>
    /// Interaction logic for BinControl.xaml
    /// </summary>
    public partial class BinControl : UserControl
    {
        public bool ShowFiller = true;
        public bool ShowAsElement = false;

        public static DependencyProperty FreeSpaceProperty = DependencyProperty.Register(
            "FreeSpace", typeof(string), typeof(BinControl));
        public string FreeSpace
        {
            get
            {
                return (string)GetValue(FreeSpaceProperty);
            }
            set
            {
                SetValue(FreeSpaceProperty, value);
            }
        }

        private bool showScaled = false;
        public bool ShowScaled
        {
            get
            {
                return showScaled;
            }
            set
            {
                showScaled = value;

                if (bin != null)
                {
                    if (showScaled)
                        FreeSpace = (bin != null) ? ((double)bin.FreeSpace() / bin.Size).ToString("0.####") : "";
                    else
                        FreeSpace = (bin != null) ? bin.FreeSpace().ToString() : "";

                    UpdateGrid();
                }
            }
        }


        private Bin bin;
        public Bin Bin
        {
            get
            {
                return bin;
            }
            set
            {
                bin = value;

                if (ShowScaled)
                    FreeSpace = (bin != null) ? ((double)bin.FreeSpace() / bin.Size).ToString("0.####") : "";
                else
                    FreeSpace = (bin != null) ? bin.FreeSpace().ToString() : "";

                UpdateGrid();
            }
        }

        public BinControl()
        {
            InitializeComponent();
            Bin = new Bin();
        }

        public void UpdateGrid()
        {
            gElements.Children.Clear();
            gElements.RowDefinitions.Clear();

            if (bin.FreeSpace() > 0)
            {
                RowDefinition rowDefinition = new RowDefinition();
                if (bin.Elements.Count > 0)
                    rowDefinition.Height = new GridLength((double)bin.FreeSpace() / bin.Size, GridUnitType.Star);
                gElements.RowDefinitions.Add(rowDefinition);

                if (ShowFiller)
                {
                    Border border = new Border();
                    border.BorderBrush = Brushes.Black;

                    if (bin.Elements.Count > 0)
                        border.BorderThickness = new Thickness(0, 3, 0, 0);
                    else
                        border.BorderThickness = new Thickness(0, 3, 0, 3);
                    gElements.Children.Add(border);
                }
            }

            for (int i = 0; i < bin.Elements.Count; i++)
            {
                int elem = bin.Elements[(bin.Elements.Count - 1) - i];

                RowDefinition rowDefinition = new RowDefinition()
                {
                    Height = new GridLength((double)elem / bin.Size, GridUnitType.Star)
                };
                gElements.RowDefinitions.Add(rowDefinition);

                ElementControl newElemControl = new ElementControl(elem);
                if (ShowAsElement)
                    newElemControl.Border.BorderThickness = new Thickness(3);
                else if (i == bin.Elements.Count - 1)
                    newElemControl.Border.BorderThickness = new Thickness(0, 3, 0, 3);

                if (ShowScaled)
                    newElemControl.Message = ((double)elem / Bin.Size).ToString("0.####");
                else
                    newElemControl.Message = elem.ToString();

                gElements.Children.Add(newElemControl);

                if (bin.FreeSpace() > 0)
                    Grid.SetRow(newElemControl, i + 1);
                else
                    Grid.SetRow(newElemControl, i);
            }
        }

        public void StartAnimation()
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.From = 1.0;
            doubleAnimation.To = 0.0;
            doubleAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            doubleAnimation.AutoReverse = true;
            doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

            Border.BeginAnimation(Border.OpacityProperty, doubleAnimation);
        }

        public void StopAnimation()
        {
            Border.BeginAnimation(Border.OpacityProperty, null);
        }
    }
}
