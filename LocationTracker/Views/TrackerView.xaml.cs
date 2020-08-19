using LocationTracker.Contracts;
using LocationTracker.Helpers;
using LocationTracker.Validators;
using LocationTracker.Views;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LocationTracker
{
    /// <summary>
    /// Code behind for TrackerView.xaml
    /// </summary>
    public partial class TrackerView : Window, IView, IGraphicView
    {
        /// <summary>
        /// Gets an factory to create specific Tracker
        /// </summary>
        public ITrackerFactory TrackerFactory { get; } = new TrackerFactory();

        /// <summary>
        /// Gets an specific Tracker
        /// </summary>
        public ITracker Tracker { get; }

        /// <summary>
        /// Gets an Validator
        /// </summary>
        public IValidator Validator { get; } = new DefaultValidator();

        /// <summary>
        /// Gets an ViewHelper
        /// </summary>
        protected ViewHelper ViewHelper { get; } = new ViewHelper();

        /// <summary>
        /// Gets or sets coefficient between X axis values and pixels they take
        /// </summary>
        public double XCoefficient { get; set; }

        /// <summary>
        /// Gets or sets coefficient between Y axis values and pixels they take
        /// </summary>
        public double YCoefficient { get; set; }

        /// <summary>
        /// Gets or sets Collection of UIElements to do not clear them from UI
        /// </summary>
        protected HashSet<UIElement> SkipDeletingElements { get; set; } = new HashSet<UIElement>();

        /// <summary>
        /// ctor
        /// </summary>
        public TrackerView()
        {
            InitializeComponent();
            Tracker = TrackerFactory.CreateFactory();
            this.Loaded += TrackerView_Loaded;
        }

        /// <summary>
        /// Method called after UI of the view loaded
        /// </summary>
        protected virtual void TrackerView_Loaded(object sender, RoutedEventArgs e)
        {
            DrawTrajectoryBtn.Visibility = Visibility.Hidden;
            InputFileOpenBtn.IsEnabled = false;
            SaveTrajectoryBtn.IsEnabled = false;
            DrawFixedValuesLines();
            RecordEmptyField();
        }

        /// <summary>
        /// Records into SkipDeletingElements property starting UI elements
        /// </summary>
        protected virtual void RecordEmptyField()
        {
            var fieldsToRecognize = DrawFieldPanel.Children;
            foreach (UIElement field in fieldsToRecognize)
            {
                SkipDeletingElements.Add(field);
            }
        }

        /// <summary>
        /// Draws short cutoff lines at the graphic's axis
        /// </summary>
        public virtual void DrawFixedValuesLines()
        {
            var fieldWidth = DrawFieldPanel.Width;
            var fieldHeight = DrawFieldPanel.Height;

            // Y axis sections
            for (int i = 80; i < fieldHeight; i += 80)
            {
                DrawFieldPanel.Children.Add(
                    new Line
                    {
                        X1 = 490,
                        X2 = 510,
                        Y1 = i,
                        Y2 = i,
                        Stroke = Brushes.Black
                    });
            }

            // X axis sections
            for (int j = 100; j < fieldWidth; j += 100)
            {
                DrawFieldPanel.Children.Add(
                    new Line
                    {
                        X1 = j,
                        X2 = j,
                        Y1 = 310,
                        Y2 = 330,
                        Stroke = Brushes.Black
                    });
            }
        }

        /// <summary>
        /// Handles clicking at the InputFileChooseBtn button
        /// </summary>
        protected virtual void InputFileChooseBtn_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                InputFilePathTxtBox.Text = openFileDialog.FileName;
                InputFileOpenBtn.IsEnabled = true;
            }
        }

        /// <summary>
        /// Handles clicking at the InputFileOpenBtn button
        /// </summary>
        protected virtual void InputFileOpenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(InputFilePathTxtBox.Text) || !Validator.ValidateInputFile(InputFilePathTxtBox.Text))
            {
                MessageBox.Show("Incorrect input data. Please check input file");
                return;
            }
            DrawTrajectoryBtn.Visibility = Visibility.Visible;
            ClearField();
        }

        /// <summary>
        /// Handles clicking at the DrawTrajectoryBtn button
        /// </summary>
        protected virtual void DrawTrajectoryBtn_Click(object sender, RoutedEventArgs e)
        {
            //var inputLines = File.ReadAllLines("\\..\\Output.txt");
            //var points = new List<TwoDimensialPoint>();
            //inputLines.ToList().ForEach(l => points.Add(new TwoDimensialPoint { Location = l }));
            var points = Tracker.GetTrajectory(InputFilePathTxtBox.Text);
            if (points.Count() == 0)
            {
                MessageBox.Show("Trajectory cannot be drawen");
                return;
            }

            var maxXValue = points.OfType<TwoDimensialPoint>().Select(p => Math.Abs(p.XPosition)).Max();
            var maxYValue = points.OfType<TwoDimensialPoint>().Select(p => Math.Abs(p.YPosition)).Max();

            // Coefficient between max value and pixels
            // Field's width and height are hardcoded now so 800 pixels is an effective coefficient for whole X axis
            XCoefficient = 800 / 2 / maxXValue;
            XAxisSectionValueLbl.Text = $"{Math.Round(maxXValue / 4, 2)}";

            // And 640 pixels is for Y axis
            YCoefficient = 640 / 2 / maxYValue;
            YAxisSectionValueLbl.Text = $"{Math.Round(maxYValue / 4, 2)}";

            var recalculatedPoints = ViewHelper.RecalculatePoints(points.OfType<TwoDimensialPoint>(), XCoefficient, YCoefficient);

            ViewHelper.DrawRecalculatedTrajectory(DrawFieldPanel,recalculatedPoints);
            SaveTrajectoryBtn.IsEnabled = true;
        }

        /// <summary>
        /// Handles clicking at the SaveTrajectoryBtn button
        /// </summary>
        protected virtual void SaveTrajectoryBtn_Click(object sender, RoutedEventArgs e)
        {
            var saveDialogFile = new SaveFileDialog();
            if (saveDialogFile.ShowDialog() == true)
            {
                Tracker.SaveOutputData(saveDialogFile.FileName);
            }
        }

        /// <summary>
        /// Handles clicking at the SimulatorRunBtn button
        /// </summary>
        protected virtual void SimulatorRunBtn_Click(object sender, RoutedEventArgs e)
        {
            var simulatorWindow = new TrackerSimulatorView();
            Hide();
            simulatorWindow.Show();
            simulatorWindow.Closed += SimulatorWindow_Closed;
        }

        /// <summary>
        /// Handles clicking at the SimulatorWindow button
        /// </summary>
        protected virtual void SimulatorWindow_Closed(object sender, EventArgs e)
        {
            Show();
        }

        /// <summary>
        /// Clears drawen trajectory from graphic field
        /// </summary>
        protected virtual void ClearField()
        {
            var drawPanelElements = DrawFieldPanel.Children.ToList();
            foreach (var field in drawPanelElements)
            {
                if (!SkipDeletingElements.Contains(field))
                {
                    DrawFieldPanel.Children.Remove(field);
                }
            }
        }
    }
}
