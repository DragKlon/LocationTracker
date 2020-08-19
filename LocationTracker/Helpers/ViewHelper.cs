using LocationTracker.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LocationTracker.Helpers
{
    /// <summary>
    /// Helps Views with graphics drawing
    /// </summary>
    public class ViewHelper
    {
        /// <summary>
        /// Recalculates points and returns a list of points with theirs X and Y positions in UI pixels for drawable panel 1000 * 640 pixels
        /// </summary>
        public virtual List<DrawablePoint> RecalculatePoints(IEnumerable<TwoDimensialPoint> points, double xCoefficient, double yCoefficient)
        {
            var recalculatedPoints = new List<DrawablePoint>();
            points.OfType<TwoDimensialPoint>().ToList().ForEach(p => recalculatedPoints.Add(new DrawablePoint(             
                    p,
                    p.XPosition >= 0 ? 500 + Math.Abs(p.XPosition * xCoefficient) : 500 - Math.Abs(p.XPosition * xCoefficient),
                    p.YPosition >= 0 ? 320 + Math.Abs(p.YPosition * yCoefficient) : 320 - Math.Abs(p.YPosition * yCoefficient)
                )));

            return recalculatedPoints;
        }

        /// <summary>
        /// Draws points trajectory
        /// </summary>
        public virtual void DrawRecalculatedTrajectory(Canvas sourceCanvas, List<DrawablePoint> recalculatedPoints)
        {
            int pointsNumber = recalculatedPoints.Count;
            for (int i = 0; i < pointsNumber; i++)
            {
                var addingPoint = new Ellipse
                {
                    Width = 5,
                    Height = 5,
                    Fill = Brushes.Black
                };

                sourceCanvas.Children.Add(addingPoint);
                Canvas.SetLeft(addingPoint, recalculatedPoints[i].RecalculatedX);
                Canvas.SetBottom(addingPoint, recalculatedPoints[i].RecalculatedY);

                if (i > 0)
                {
                    var connectingLine = new Line
                    {
                        X1 = recalculatedPoints[i - 1].RecalculatedX,
                        Y1 = sourceCanvas.Height - recalculatedPoints[i - 1].RecalculatedY,
                        X2 = recalculatedPoints[i].RecalculatedX,
                        Y2 = sourceCanvas.Height - recalculatedPoints[i].RecalculatedY,
                        Stroke = Brushes.Black
                    };
                    sourceCanvas.Children.Add(connectingLine);
                }
            }
        }
    }
}
