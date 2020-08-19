using LocationTracker.Contracts;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LocationTracker.Helpers
{
    /// <summary>
    /// Class to extend string type with ToInputData method
    /// </summary>
    public static class StringToInputDataExtension
    {
        /// <summary>
        /// Converts input file's string to InputData
        /// </summary>
        /// <param name="inputStrings">Collection of string to convert</param>
        /// <param name="trackerType">Specific Tracker type</param>
        /// <returns>Specific IInputData implementation for the trackerType</returns>
        public static IInputData ToInputData(this IEnumerable<string> inputStrings, TrackerTypes trackerType = TrackerTypes.TwoDimensial)
        {
            switch (trackerType)
            {
                case TrackerTypes.TwoDimensial:
                    return TwoDimensialConverter(inputStrings);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Converts input file's string to InputData for two dimensial Tracker
        /// </summary>
        /// <param name="inputStrings">Collection of string to convert</param>
        private static InputData TwoDimensialConverter(IEnumerable<string> inputStrings)
        {
            string[] inputStringsArray = inputStrings.ToArray();
            var inputLinesCount = inputStringsArray.Length;
            var propagationTimes = new List<List<double>>();
            InputData inputData = new InputData
            {
                Receivers = inputStrings.FirstOrDefault()?.ToReceivers(PublicFields.PositionSeparator)
            };

            // first line is receivers line so start from second line
            for (int i = 1; i < inputLinesCount; i++)
            {
                var propagationTimesStrings = inputStringsArray[i].Split(PublicFields.PositionSeparator);
                var timeToFirstReceiver = double.Parse(propagationTimesStrings[0], CultureInfo.InvariantCulture);
                var timeToSecondReceiver = double.Parse(propagationTimesStrings[1], CultureInfo.InvariantCulture);
                var timeToThirdReceiver = double.Parse(propagationTimesStrings[2], CultureInfo.InvariantCulture);
                propagationTimes.Add(new List<double> { timeToFirstReceiver, timeToSecondReceiver, timeToThirdReceiver });
            }

            inputData.PropagationTime = propagationTimes;

            return inputData;
        }
    }
}
