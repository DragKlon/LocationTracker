using LocationTracker.Validators;
using System.Collections.Generic;
using System.Globalization;

namespace LocationTracker.Contracts
{
    /// <summary>
    /// Two-dimensial Point class
    /// </summary>
    public class ThreeDimensialPoint : IPoint
    {
        /// <summary>
        /// Gets or sets TwoDimensialPoint's position in X ort
        /// </summary>
        public double XPosition { get; set; }

        /// <summary>
        /// Gets or sets TwoDimensialPoint's position in Y ort
        /// </summary>
        public double YPosition { get; set; }

        /// <summary>
        /// Gets or sets TwoDimensialPoint's position in Z ort
        /// </summary>
        public double ZPosition { get; set; }

        /// <summary>
        /// Collection of different dimension positions
        /// </summary>
        public IEnumerable<double> Positions { get => new List<double> { XPosition, YPosition, ZPosition }; set => Positions = value; }

        /// <summary>
        /// Gets or sets ThreeDimensialPoint's location as string
        /// </summary>
        public string Location
        {
            get => $"{XPosition.ToString("0.00000000", CultureInfo.InvariantCulture)}{PublicFields.PositionSeparator}{YPosition.ToString("0.00000000", CultureInfo.InvariantCulture)}" +
                $"{PublicFields.PositionSeparator}{ZPosition.ToString("0.00000000", CultureInfo.InvariantCulture)}";
            set => Location = value;
        }
    }
}
