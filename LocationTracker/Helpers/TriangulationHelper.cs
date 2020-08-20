using LocationTracker.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LocationTracker.Helpers
{
    /// <summary>
    /// Helps with finding position using triangulation basics
    /// </summary>
    public class TriangulationHelper
    {
        /// <summary>
        /// Gets an instance of ConvertingHelper
        /// </summary>
        protected ConvertingHelper ConvertingHelper { get; } = new ConvertingHelper();

        /// <summary>
        /// Returns position for given signals with propagation times to receivers
        /// </summary>
        /// <param name="receivers">Receivers collection</param>
        /// <param name="propagationTimes">Propagation times collection</param>
        /// <param name="error">Optional: measurment error. By default 0.</param>
        /// <returns>Position or null if it is impossible to find position</returns>
        public virtual IPoint GetPosition(IEnumerable<IPoint> receivers, IEnumerable<double> propagationTimes, int error = 0)
        {
            if (receivers.All(r => r is TwoDimensialPoint))
            {
                return GetTwoDimensialPosition(receivers.OfType<TwoDimensialPoint>(), propagationTimes, error);
            }

            return null;
        }

        /// <summary>
        /// Returns two dimensial position for signals with propagation times to receivers
        /// </summary>
        /// <param name="receivers">Receivers collection</param>
        /// <param name="propagationTimes">Propagation times collection</param>
        /// <returns>Two dimensial position or null if it is impossible to find position</returns>
        protected virtual TwoDimensialPoint GetTwoDimensialPosition(IEnumerable<TwoDimensialPoint> receivers, IEnumerable<double> propagationTimes, int error)
        {
            // Between receiver signal's circles there might be 2 intersection
            // so it is necessary to find common intersection point for 2 receiver pairs 
            var radiusesArray = ConvertingHelper.ConvertTimesToRadiuses(propagationTimes).ToArray();
            var receiversArray = receivers.ToArray();
            var firstPairIntersections = TwoCirclesIntersection(receiversArray[0], receiversArray[1], radiusesArray[0], radiusesArray[1]);
            var secondPairIntersections = TwoCirclesIntersection(receiversArray[1], receiversArray[2], radiusesArray[1], radiusesArray[2]);
            var thirdPairIntersections = TwoCirclesIntersection(receiversArray[0], receiversArray[2], radiusesArray[0], radiusesArray[2]);

            // If there is no intersections try to find them using an measurment error for each pair of circles
            if (firstPairIntersections.Count() == 0 && error != 0)
            {
                firstPairIntersections = TwoCirclesIntersection(receiversArray[0], receiversArray[1], radiusesArray[0], radiusesArray[1], error);
            }

            if (secondPairIntersections.Count() == 0 && error != 0)
            {
                secondPairIntersections = TwoCirclesIntersection(receiversArray[1], receiversArray[2], radiusesArray[1], radiusesArray[2], error);
            }

            if (thirdPairIntersections.Count() == 0 && error != 0)
            {
                thirdPairIntersections = TwoCirclesIntersection(receiversArray[0], receiversArray[2], radiusesArray[2], radiusesArray[2], error);
            }

            if (firstPairIntersections.Count() != 2 || secondPairIntersections.Count() != 2 || thirdPairIntersections.Count() != 2)
                return null;

            // FindPairsIntersection will look at each intersection as at the 'spot' - points with some error radius outside them
            double firstSpotRadius = (radiusesArray[0] > radiusesArray[1] ? radiusesArray[1] : radiusesArray[0]) * PublicFields.Error / 100;
            double secondSpotRadius = (radiusesArray[1] > radiusesArray[2] ? radiusesArray[2] : radiusesArray[1]) * PublicFields.Error / 100;
            double thirdSpotRadius = (radiusesArray[0] > radiusesArray[2] ? radiusesArray[2] : radiusesArray[0]) * PublicFields.Error / 100;

            var resultPosition = FindPairsIntersection(firstPairIntersections, secondPairIntersections, thirdPairIntersections, firstSpotRadius, secondSpotRadius, thirdSpotRadius);

            return resultPosition;
        }

        /// <summary>
        /// Finds common point for each pair of intersection when each intersection presented as spot with center position at the intersection and with spot radius depending on measurment error
        /// </summary>
        protected virtual TwoDimensialPoint FindPairsIntersection(IEnumerable<TwoDimensialPoint> firstPairIntersections, IEnumerable<TwoDimensialPoint> secondPairIntersections, IEnumerable<TwoDimensialPoint> thirdPairIntersections
            , double firstSpotRadius, double secondSpotRadius, double thirdSpotRadius)
        {
            foreach (var firstPoint in firstPairIntersections)
            {
                foreach (var secondPoint in secondPairIntersections)
                {
                    foreach (var thirdPoint in thirdPairIntersections)
                    {
                        bool firstAndSecondAreIntersect = TwoCirclesIntersection(firstPoint, secondPoint, firstSpotRadius, secondSpotRadius).Count() == 2;
                        if (firstAndSecondAreIntersect)
                        {
                            return firstPoint;
                        }

                        bool firstAndThirdAreIntersect = TwoCirclesIntersection(firstPoint, thirdPoint, firstSpotRadius, thirdSpotRadius).Count() == 2;
                        if (firstAndThirdAreIntersect)
                        {
                            return firstPoint;
                        }

                        bool secondAndThirdAreIntersect = TwoCirclesIntersection(secondPoint, thirdPoint, secondSpotRadius, thirdSpotRadius).Count() == 2;
                        if (secondAndThirdAreIntersect)
                        {
                            return secondPoint;
                        }
                    }

                }
            }

            return null;
        }

        /// <summary>
        /// Finds two circles intersection
        /// </summary>
        /// <param name="firstCenter">First circle's center</param>
        /// <param name="secondCenter">Second circle's center</param>
        /// <param name="r1">First circle's radius</param>
        /// <param name="r2">Second circle's radius</param>
        /// <returns>Collection of Points where cirles are intersect or empty collections when there is no intercesctions</returns>
        protected virtual IEnumerable<TwoDimensialPoint> TwoCirclesIntersection(TwoDimensialPoint firstCenter, TwoDimensialPoint secondCenter, double r1, double r2)
        {
            var intersections = new List<TwoDimensialPoint>();
            var a1 = firstCenter.XPosition;
            var b1 = firstCenter.YPosition;
            var a2 = secondCenter.XPosition;
            var b2 = secondCenter.YPosition;

            #region MATH THEORY
            //
            // Circle equation with radius R and center placed in point (a,b):
            // (x-a)^2 + (y-b)^2 = R^2
            // 
            // Two circles will have common x and y values if they are intersect.
            // So we can create next system of equation for cirlces,
            // where a1 and b1 - position of first circle's center with radius R1
            // and a2 and b2 - position of second circle's center with radius R2
            //
            // { (x-a1)^2 + (y-b1)^2 = R1^2
            // { (x-a2)^2 + (y-b2)^2 = R2^2
            // 
            // Open brackets and get:
            // 
            // { x^2 - 2*a1*x + a1^2 + y^2 - 2*b1*y + b1^2 = R1^2
            // { x^2 - 2*a2*x + a2^2 + y^2 - 2*b2*y + b2^2 = R2^2
            // 
            // Lets subtract second from the first:
            // 
            // - 2*a1*x + 2*a2*x + a1^2 - a2^2 - 2*b1*y + 2*b2*y + b1^2 - b2^2 = R1^2 - R2^2
            // 
            // Express x through y: 
            // 
            // x = (R1^2 - R2^2 - a1^2 + a2^2 - b1^2 + b2^2)/(2*(a2 - a1)) + y*(b1 - b2)/(a2 - a1)
            // 
            // Lets make a replacement
            // new coefficient K = (R1^2 - R2^2 - a1^2 + a2^2 - b1^2 + b2^2)/(2*(a2 - a1))
            // 
            // Then replace x with equation above into "(x-a2)^2 + (y-b2)^2 = R2^2" and get:
            // 
            // K^2 + y^2*((b1 - b2)/(a2 - a1))^2 + y*2*K*(b1 - b2)/(a2 - a1) - 2*a2*K - y*2*a2*(b1 - b2)/(a2 - a1) + a2^2 + y^2 - y*2*b2 + b2^2 = R2^2
            // 
            // Calculate and join coefficients for each degree of y:
            // 
            // y^2*(((b1 - b2)/(a2 - a1))^2 + 1) + y*(2*K*(b1 - b2)/(a2 - a1) - 2*a2*(b1 - b2)/(a2 - a1)-2*b2) + (K^2 - R2^2 - 2*a2*K + a2^2 + b2^2) = 0
            // 
            // Lets make next replacements to get equation with view "y^2*A + y*B + C = 0":
            // A = ((b1 - b2)/(a2 - a1))^2 + 1
            // B = 2*K*(b1 - b2)/(a2 - a1) - 2*a2*(b1 - b2)/(a2 - a1)-2*b2
            // C = K^2 - R2^2 - 2*a2*K + a2^2 + b2^2
            // 
            // Discriminant D for such type of equation is equal:
            // D = B^2 - 4*A*C
            // 
            // So, roots of equation are:
            // y1 = (-B + Sqrt(D))/(2*A)
            // y2 = (-B - Sqrt(D))/(2*A)
            // 
            // Substite this values in expression x through y and get:
            // x1 = K + y1*(b1 - b2)/(a2 - a1)
            // x2 = K + y2*(b1 - b2)/(a2 - a1)
            //
            // So we get 2 points of intersection (x1,y1) and (x2,y2)
            // 
            #endregion

            // Coefficent K
            double K = (r1.Square() - r2.Square() - a1.Square() + a2.Square() - b1.Square() + b2.Square()) / (2 * (a2 - a1));

            // Coefficent A
            double A = ((b1 - b2) / (a2 - a1)).Square() + 1;

            // Coefficent B
            double B = 2 * K * ((b1 - b2) / (a2 - a1)) - 2 * a2 * ((b1 - b2) / (a2 - a1)) - 2 * b2;

            // Coefficent C
            double C = K.Square() - r2.Square() - 2 * a2 * K + a2.Square() + b2.Square();

            // Quadratic discriminant
            double discriminant = Math.Sqrt(B.Square() - 4 * A * C);

            // If there is no intersections discriminant will be NaN
            if (double.IsNaN(discriminant))
                return intersections;

            double Y1 = ((-1) * B + discriminant) / (2 * A);

            TwoDimensialPoint firstIntersection = new TwoDimensialPoint
            {
                YPosition = Math.Round(Y1, 8),
                XPosition = Math.Round(K + Y1 * ((b1 - b2) / (a2 - a1)), 8)
            };

            double Y2 = ((-1) * B - discriminant) / (2 * A);

            TwoDimensialPoint secondIntersection = new TwoDimensialPoint
            {
                YPosition = Math.Round(Y2, 8),
                XPosition = Math.Round(K + Y2 * ((b1 - b2) / (a2 - a1)), 8)
            };

            intersections.AddRange(new List<TwoDimensialPoint> { firstIntersection, secondIntersection });

            return intersections;
        }

        /// <summary>
        /// Finds two circles intersection according with measurment error
        /// </summary>
        /// <param name="firstCenter">First circle's center</param>
        /// <param name="secondCenter">Second circle's center</param>
        /// <param name="r1">First circle's radius</param>
        /// <param name="r2">Second circle's radius</param>
        /// <returns>Collection of Points where cirles are intersect or empty collections when there is no intercesctions</returns>
        protected virtual IEnumerable<TwoDimensialPoint> TwoCirclesIntersection(TwoDimensialPoint firstCenter, TwoDimensialPoint secondCenter, double r1, double r2, int error)
        {
            double r1Minimum = r1 - r1 * error / 100;
            double r1Maximum = r1 + r1 * error / 100;
            double r2Minimum = r2 - r2 * error / 100;
            double r2Maximum = r2 + r2 * error / 100;

            var resultIntersections = TwoCirclesIntersection(firstCenter, secondCenter, r1Minimum, r2Minimum);
            if (resultIntersections.Count() == 2)
                return resultIntersections;

            resultIntersections = TwoCirclesIntersection(firstCenter, secondCenter, r1Minimum, r2Maximum);
            if (resultIntersections.Count() == 2)
                return resultIntersections;

            resultIntersections = TwoCirclesIntersection(firstCenter, secondCenter, r1Maximum, r2Minimum);
            if (resultIntersections.Count() == 2)
                return resultIntersections;

            resultIntersections = TwoCirclesIntersection(firstCenter, secondCenter, r1Maximum, r2Maximum);

            return resultIntersections;
        }

    }
}
