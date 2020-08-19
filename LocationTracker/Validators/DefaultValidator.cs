using LocationTracker.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;

namespace LocationTracker.Validators
{
    /// <summary>
    /// Classifies a default validator
    /// </summary>
    public class DefaultValidator : IValidator
    {
        /// <summary>
        /// Checks if input data has correct content
        /// </summary>
        /// <param name="inputPath">Input data file path</param>
        /// <param name="trackerType">Which tracker type will work with data</param>
        public virtual bool ValidateInputFile(string inputPath, TrackerTypes trackerType = TrackerTypes.TwoDimensial)
        {
            switch (trackerType)
            {
                case TrackerTypes.TwoDimensial:
                    return ValidateTwoDimensialInputFile(inputPath);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks if input data has correct content for two dimensial tracker
        /// </summary>
        /// <param name="inputPath">Input data file path</param>
        protected virtual bool ValidateTwoDimensialInputFile(string inputPath)
        {
            var lines = ReadAllLines(inputPath);
            int linesNumber = lines.Count();

            // If there is lines number less than minimum number
            if (linesNumber < 2)
                return false;

            // If first line is incorrect as line with receivers
            var receivers = lines.FirstOrDefault();
            if (!ValidateTwoDimensialReceivers(receivers))
                return false;

            // Check all lines
            for (int i = 1; i < linesNumber; i++)
            {
                if (!ValidatePropagationTimesLine(lines.ToArray()[i]))
                {
                    return false;
                }
            }

            return true;
        }

        protected virtual string[] ReadAllLines(string inputPath)
        {
            return File.ReadAllLines(inputPath);
        }

        /// <summary>
        /// Checks is line has propagation times in correct format
        /// </summary>
        /// <param name="line">Line containg times</param>
        protected virtual bool ValidatePropagationTimesLine(string line)
        {
            var propagationTimes = line.Split(PublicFields.PositionSeparator);

            // If incorrect number of times at the line
            if (propagationTimes.Count() != 3)
                return false;

            // If times are not presented as double
            if (propagationTimes.Any(t => !double.TryParse(t, NumberStyles.Float, CultureInfo.InvariantCulture, out double d)))
                return false;

            return true;
        }

        /// <summary>
        /// Checks is line has receiver's positions in correct format
        /// </summary>
        /// <param name="receivers">String containg receiver's positions</param>
        public virtual bool ValidateTwoDimensialReceivers(string receivers)
        {
            var firstLinePositions = receivers.Split(PublicFields.PositionSeparator);

            // If positions number is not correct for number of 2d Points
            if (firstLinePositions.Count() % 2 != 0)
                return false;

            // If receivers positions are not presented as double
            if (firstLinePositions.Any(p => !double.TryParse(p, NumberStyles.Float, CultureInfo.InvariantCulture, out double d)))
                return false;

            return true;
        }

        /// <summary>
        /// Checks does input file exists
        /// </summary>
        /// <param name="path">Input file path</param>
        /// <returns>True - if exists, False - if not exists</returns>
        public virtual bool ValidateInputPath(string path)
        {
            return File.Exists(path);
        }


        /// <summary>
        /// Validates does location is presented correctly for the tracker type
        /// </summary>
        /// <param name="location">Location presented as string</param>
        /// <param name="trackerType">TrackerTypes.TwoDimensial be deafault</param>
        public virtual bool ValidateLocation(string location, TrackerTypes trackerType = TrackerTypes.TwoDimensial)
        {
            switch (trackerType)
            {
                case TrackerTypes.TwoDimensial:
                    return ValidateTwoDimensialLocation(location);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Checks does any file already exists for the path
        /// </summary>
        /// <param name="path">Output file path</param>
        /// <returns>True - if not exists, False - if exists</returns>
        public virtual bool ValidateOutputPath(string path)
        {
            return !File.Exists(path);
        }

        /// <summary>
        /// Checks does location presented as string is correct for 2d Point
        /// </summary>
        /// <param name="location"></param>
        protected virtual bool ValidateTwoDimensialLocation(string location)
        {
            var positions = location.Split(PublicFields.PositionSeparator);
            if (positions.Length == 2)
            {
                // If any string position exists which cannot be parsed to double
                bool positionParsingError = positions.Any(p => !double.TryParse(p, NumberStyles.Float, CultureInfo.InvariantCulture, out double d));
                return !positionParsingError;
            }
            return false;
        }
    }
}
