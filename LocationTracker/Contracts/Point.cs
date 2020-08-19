using LocationTracker.Validators;
using System.Collections.Generic;
using System.Globalization;

namespace LocationTracker.Contracts
{
    /// <summary>
    /// Two-dimensial Point class
    /// </summary>
    public class TwoDimensialPoint : IPoint
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
        /// Is used to validate some values
        /// </summary>
        protected IValidator validator;

        /// <summary>
        /// ctor
        /// </summary>
        public TwoDimensialPoint()
        {
            validator = new DefaultValidator();
        }

        /// <summary>
        /// Gets TwoDimensialPoint's location as string
        /// Sets XPosition and YPosition if they are presented correctly as a string
        /// </summary>
        public string Location
        {
            get => $"{XPosition.ToString("0.00000000",CultureInfo.InvariantCulture)}{PublicFields.PositionSeparator}{YPosition.ToString("0.00000000", CultureInfo.InvariantCulture)}";
            set
            {
                if (validator.ValidateLocation(value))
                {
                    var positions = value.Split(',');
                    double.TryParse(positions[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double locationsXPosition);
                    double.TryParse(positions[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double locationsYPosition);
                    XPosition = locationsXPosition;
                    YPosition = locationsYPosition;
                }
            }
        }

        /// <summary>
        /// Collection of different dimension positions
        /// </summary>
        public IEnumerable<double> Positions { get => new List<double> { XPosition, YPosition }; set => Positions = value; }

    }
}
