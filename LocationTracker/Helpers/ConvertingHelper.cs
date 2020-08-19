using LocationTracker.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LocationTracker.Helpers
{
    /// <summary>
    /// Helps with converting from one type to another
    /// </summary>
    public class ConvertingHelper
    {
        /// <summary>
        /// Converts points to propogation times
        /// </summary>
        /// <param name="receivers">Receivers point</param>
        /// <param name="points">Trajectory points</param>
        public virtual IEnumerable<IEnumerable<double>> ConvertPointsToPropagationTimes(IEnumerable<IPoint> receivers, IEnumerable<IPoint> points)
        {
            var propogationTimes = new List<IEnumerable<double>>();
            if (receivers.All(r => r is TwoDimensialPoint) && points.All(p => p is TwoDimensialPoint))
            {
                propogationTimes = ConvertTwoDimensialPointsToPropagationTimes(receivers.OfType<TwoDimensialPoint>(), points.OfType<TwoDimensialPoint>()).ToList();
            }

            return propogationTimes;
        }

        /// <summary>
        /// Converts points to propogation times
        /// </summary>
        /// <param name="receivers">Receivers point</param>
        /// <param name="points">Trajectory points</param>
        protected virtual IEnumerable<IEnumerable<double>> ConvertTwoDimensialPointsToPropagationTimes(IEnumerable<TwoDimensialPoint> receivers, IEnumerable<TwoDimensialPoint> points)
        {
            var propogationTimes = new List<IEnumerable<double>>();

            foreach (var point in points)
            {
                var radiuses = new List<double>();
                foreach (var receiver in receivers)
                {
                    radiuses.Add(
                        Math.Sqrt(Math.Pow(receiver.XPosition - point.XPosition, 2) + Math.Pow(receiver.YPosition - point.YPosition, 2)));
                }

                propogationTimes.Add(ConvertRadiusesToPropogationTimes(radiuses));
            }

            return propogationTimes;
        }

        /// <summary>
        /// Converts distances to propagation times
        /// </summary>
        public virtual IEnumerable<double> ConvertRadiusesToPropogationTimes(List<double> radiuses)
        {
            var times = new List<double>();
            radiuses.ForEach(r => times.Add(r / PublicFields.WavesSpeed));
            return times;
        }

        /// <summary>
        /// Converts propagation times to distances
        /// </summary>
        public virtual IEnumerable<double> ConvertTimesToRadiuses(IEnumerable<double> propagationTimes)
        {
            List<double> radiuses = new List<double>();
            propagationTimes.ToList().ForEach(t => radiuses.Add(t * PublicFields.WavesSpeed));
            return radiuses;
        }
    }
}
