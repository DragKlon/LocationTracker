using LocationTracker.Contracts;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LocationTracker.Helpers
{
    /// <summary>
    /// Class to extend string type with ToReceivers method
    /// </summary>
    public static class StringToReceiversExtensions
    {
        /// <summary>
        /// Converts string line to the collection of Points
        /// </summary>
        /// <param name="firstLine">Points presented as string</param>
        /// <param name="separator">Char which separate point's positions between themselves at the line</param>
        /// <param name="trackerType">Specific Tracker type</param>
        /// <returns>Collection of points</returns>
        public static IEnumerable<IPoint> ToReceivers(this string firstLine, char separator, TrackerTypes trackerType = TrackerTypes.TwoDimensial)
        {
            switch (trackerType)
            {
                case TrackerTypes.TwoDimensial:
                    return TwoDimensialConverter(firstLine, separator);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Converts string line to the collection of two dimensial Points
        /// </summary>
        /// <param name="firstLine">Points presented as string</param>
        /// <param name="separator">Char which separate point's positions between themselves at the line</param>
        /// <returns>Collection of points</returns>
        private static IEnumerable<IPoint> TwoDimensialConverter(string firstLine, char separator)
        {
            var positionsStrings = firstLine.Split(separator);
            List<double> positions = new List<double>();

            positionsStrings.ToList().ForEach(s =>
            {
                double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double position);
                positions.Add(position);
            });

            List<IPoint> receivers = new List<IPoint>
            {
                new TwoDimensialPoint { XPosition = positions[0], YPosition = positions[1] },
                new TwoDimensialPoint { XPosition = positions[2], YPosition = positions[3] },
                new TwoDimensialPoint { XPosition = positions[4], YPosition = positions[5] }
            };

            return receivers;
        }
    }
}
