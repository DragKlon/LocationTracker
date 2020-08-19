using LocationTracker.Contracts;
using LocationTracker.Contracts.Interfaces;
using LocationTracker.Helpers;
using LocationTracker.Validators;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LocationTracker.Views
{
    /// <summary>
    /// Code behind for TrackerSimulatorView.xaml
    /// </summary>
    public partial class TrackerSimulatorView : Window, IValidatingView, IGraphicView
    {
        /// <summary>
        /// Gets the specific validator
        /// </summary>
        public IValidator Validator { get; } = new DefaultValidator();

        /// <summary>
        /// Gets an ViewHelper entity
        /// </summary>
        protected ViewHelper ViewHelper { get; } = new ViewHelper();

        /// <summary>
        /// Gets an ConvertingHelper entity
        /// </summary>
        public ConvertingHelper ConvertingHelper { get; } = new ConvertingHelper();

        /// <summary>
        /// Gets or sets receivers points
        /// </summary>
        protected IEnumerable<IPoint> Receivers { get; set; } = new List<TwoDimensialPoint>();

        /// <summary>
        /// Gets or sets Collection of UIElements to do not clear them from UI
        /// </summary>
        protected HashSet<UIElement> SkipDeletingElements { get; set; } = new HashSet<UIElement>();

        /// <summary>
        /// Gets or sets trjectory's points
        /// </summary>
        protected List<TwoDimensialPoint> Points = new List<TwoDimensialPoint>();

        /// <summary>
        /// ctor
        /// </summary>
        public TrackerSimulatorView()
        {
            InitializeComponent();
            Loaded += TrackerSimulatorView_Loaded;
        }

        /// <summary>
        /// Method called after UI of the view loaded
        /// </summary>
        protected virtual void TrackerSimulatorView_Loaded(object sender, RoutedEventArgs e)
        {
            SaveTrajectoryBtn.IsEnabled = false;
            PreviewNewPointBtn.IsEnabled = false;
            AddNewPointBtn.IsEnabled = false;
            RemoveLastPointBtn.IsEnabled = false;
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
        /// Handles clicking at the SetReceiversBtn button
        /// </summary>
        protected virtual void SetReceiversBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder receivers = new StringBuilder();
            receivers.Append(FirstReceiverXTxt.Text).Append(PublicFields.PositionSeparator).Append(FirstReceiverYTxt.Text)
                .Append(PublicFields.PositionSeparator).Append(SecondReceiverXTxt.Text).Append(PublicFields.PositionSeparator).Append(SecondReceiverYTxt.Text)
                .Append(PublicFields.PositionSeparator).Append(ThirdReceiverXTxt.Text).Append(PublicFields.PositionSeparator).Append(ThirdReceiverYTxt.Text);

            var stringReceivers = receivers.ToString();

            if (!(Validator as DefaultValidator).ValidateTwoDimensialReceivers(stringReceivers))
            {
                MessageBox.Show("Receivers have incorrect format");
                return;
            }

            Receivers = stringReceivers.ToReceivers(PublicFields.PositionSeparator);

            PreviewNewPointBtn.IsEnabled = true;
            AddNewPointBtn.IsEnabled = true;
        }

        /// <summary>
        /// Handles clicking at the PreviewNewPointBtn button
        /// </summary>
        protected virtual void PreviewNewPointBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateNewPoint())
            {
                MessageBox.Show("New point's positions are incorrect");
                return;
            }

            var tempPointsList = new List<TwoDimensialPoint>();
            tempPointsList.AddRange(Points);
            var newPoint = new TwoDimensialPoint { Location = $"{NewPointXTxt.Text}{PublicFields.PositionSeparator}{NewPointYTxt.Text}" };
            tempPointsList.Add(newPoint);

            var maxXValue = tempPointsList.OfType<TwoDimensialPoint>().Select(p => Math.Abs(p.XPosition)).Max();
            var maxYValue = tempPointsList.OfType<TwoDimensialPoint>().Select(p => Math.Abs(p.YPosition)).Max();

            // Coefficient between max value and pixels
            // Field's width and height are hardcoded now so 800 pixels is an effective coefficient for whole X axis
            var xCoefficient = 800 / 2 / maxXValue;

            // And 600 pixels is for Y axis
            var yCoefficient = 640 / 2 / maxYValue;

            var recalculatedPoints = ViewHelper.RecalculatePoints(tempPointsList.OfType<TwoDimensialPoint>(), xCoefficient, yCoefficient);
            var recalculatedNewPoint = recalculatedPoints.FirstOrDefault(p => p.Point == newPoint);

            XAxisSectionValueLbl.Text = $"{Math.Round(maxXValue / 4, 2)}";
            YAxisSectionValueLbl.Text = $"{Math.Round(maxYValue / 4, 2)}";

            //remove new point because it was used to recalculate scales
            recalculatedPoints.Remove(recalculatedNewPoint);

            ClearField();
            ViewHelper.DrawRecalculatedTrajectory(DrawFieldPanel, recalculatedPoints);

            var newPointElement = new Ellipse
            {
                Width = 5,
                Height = 5,
                Fill = Brushes.Red
            };



            Canvas.SetLeft(newPointElement, recalculatedNewPoint.RecalculatedX);
            Canvas.SetBottom(newPointElement, recalculatedNewPoint.RecalculatedY);
            DrawFieldPanel.Children.Add(newPointElement);
        }

        /// <summary>
        /// Validates is new point's entered data has correct format to be converted to Point entity
        /// </summary>
        protected virtual bool ValidateNewPoint()
        {
            var location = $"{NewPointXTxt.Text}{PublicFields.PositionSeparator}{NewPointYTxt.Text}";
            return (Validator as DefaultValidator).ValidateLocation(location);
        }

        /// <summary>
        /// Handles clicking at the AddNewPointBtn button
        /// </summary>
        protected virtual void AddNewPointBtn_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateNewPoint())
            {
                MessageBox.Show("New point's positions are incorrect");
                return;
            }

            var newPoint = new TwoDimensialPoint { Location = $"{NewPointXTxt.Text}{PublicFields.PositionSeparator}{NewPointYTxt.Text}" };
            Points.Add(newPoint);

            RefreshDrawPanel();
            RemoveLastPointBtn.IsEnabled = true;
            SaveTrajectoryBtn.IsEnabled = true;
        }

        /// <summary>
        /// Redraws scales and a trajectory for Points collection
        /// </summary>
        protected void RefreshDrawPanel()
        {
            var maxXValue = Points.OfType<TwoDimensialPoint>().Select(p => Math.Abs(p.XPosition)).Max();
            var maxYValue = Points.OfType<TwoDimensialPoint>().Select(p => Math.Abs(p.YPosition)).Max();

            // Coefficient between max value and pixels
            // Field's width and height are hardcoded now so 800 pixels is an effective coefficient for whole X axis
            var xCoefficient = 800 / 2 / maxXValue;
            // And 640 pixels is for Y axis
            var yCoefficient = 640 / 2 / maxYValue;

            var recalculatedPoints = ViewHelper.RecalculatePoints(Points.OfType<TwoDimensialPoint>(), xCoefficient, yCoefficient);

            YAxisSectionValueLbl.Text = $"{Math.Round(maxYValue / 4, 2)}";
            XAxisSectionValueLbl.Text = $"{Math.Round(maxXValue / 4, 2)}";
            ClearField();
            ViewHelper.DrawRecalculatedTrajectory(DrawFieldPanel, recalculatedPoints);
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

        /// <summary>
        /// Handles clicking at the SaveTrajectoryBtn button
        /// </summary>
        protected virtual void SaveTrajectoryBtn_Click(object sender, RoutedEventArgs e)
        {
            var saveDialogFile = new SaveFileDialog();
            if (saveDialogFile.ShowDialog() == true)
            {
                var simulatedData = PrepareSimulatedData();
                File.WriteAllText(saveDialogFile.FileName, simulatedData);
            }
        }

        /// <summary>
        /// Prepares simulated input data as string
        /// </summary>
        protected virtual string PrepareSimulatedData()
        {
            StringBuilder inputData = new StringBuilder();
            StringBuilder tempLine = new StringBuilder();
            Receivers.ToList().ForEach(r => inputData.Append(r.Location).Append(PublicFields.PositionSeparator));
            inputData = inputData.Trim(PublicFields.PositionSeparator);
            inputData.Append('\n');
            var times = ConvertingHelper.ConvertPointsToPropagationTimes(Receivers, Points);

            foreach (var timeLine in times)
            {
                tempLine.Clear();
                foreach (var time in timeLine)
                {
                    tempLine.Append(time.ToString("0.00000000", CultureInfo.InvariantCulture)).Append(PublicFields.PositionSeparator);
                }
                tempLine = tempLine.Trim(PublicFields.PositionSeparator);
                tempLine.Append('\n');
                inputData.Append(tempLine);
            }

            return inputData.ToString();
        }

        /// <summary>
        /// Handles clicking at the RemoveLastPointBtn button
        /// </summary>
        protected void RemoveLastPointBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Points.Count == 0) return;
            int lastPointIndex = Points.Count - 1;
            Points.RemoveAt(lastPointIndex);

            if (Points.Count == 0)
            {
                RemoveLastPointBtn.IsEnabled = false;
                SaveTrajectoryBtn.IsEnabled = false;
                ClearField();
                return;
            }

            RefreshDrawPanel();
        }
    }
}
