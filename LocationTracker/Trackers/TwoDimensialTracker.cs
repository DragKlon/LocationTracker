using LocationTracker.Contracts;
using LocationTracker.Helpers;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace LocationTracker.Trackers
{
    /// <summary>
    /// Clasifies two dimensial Tracker
    /// </summary>
    class TwoDimensialTracker : ITracker
    {
        /// <summary>
        /// Gets an triangulation helper
        /// </summary>
        protected TriangulationHelper TriangulationHelper { get; }

        /// <summary>
        /// Gets or sets output data
        /// </summary>
        public IEnumerable<string> OutputData { get; set; }

        /// <summary>
        /// ctor
        /// </summary>
        public TwoDimensialTracker()
        {
            TriangulationHelper = new TriangulationHelper();
        }

        /// <summary>
        /// Returns a trajectory created from input file
        /// </summary>
        public virtual IEnumerable<IPoint> GetTrajectory(string inputFilePath)
        {
            var inputData = LoadInputData(inputFilePath) as InputData;
            var trajectoryPoints = new List<IPoint>();
            var timesCollection = inputData.PropagationTime;
            OutputData = new List<string>();
            foreach (var time in timesCollection)
            {
                var position = TriangulationHelper.GetPosition(inputData.Receivers, time, PublicFields.Error) as TwoDimensialPoint;
                if (position != null)
                {
                    (OutputData as List<string>).Add($"{position.XPosition.ToString("0.00000000", CultureInfo.InvariantCulture)}{PublicFields.PositionSeparator}{position.YPosition.ToString("0.00000000", CultureInfo.InvariantCulture)}");
                    trajectoryPoints.Add(position);
                }
            }

            return trajectoryPoints;
        }

        /// <summary>
        /// Returns an input data loaded from path
        /// </summary>
        public virtual IInputData LoadInputData(string inputFilePath)
        {
            var inputFileLines = File.ReadAllLines(inputFilePath);
            IInputData inputData = inputFileLines.ToInputData();
            return inputData;
        }

        /// <summary>
        /// Saves result to path
        /// </summary>
        public virtual void SaveOutputData(string outputFilePath)
        {
            File.WriteAllLines(outputFilePath, OutputData);
        }
    }
}
