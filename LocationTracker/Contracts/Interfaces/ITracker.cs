using System.Collections.Generic;

namespace LocationTracker.Contracts
{
    /// <summary>
    /// Classifies a specific tracker
    /// </summary>
    public interface ITracker
    {
        /// <summary>
        /// Gets or sets output data
        /// </summary>
        IEnumerable<string> OutputData { get; set; }

        /// <summary>
        /// Returns a trajectory created from input file
        /// </summary>
        IEnumerable<IPoint> GetTrajectory(string inputFilePath);

        /// <summary>
        /// Returns an input data loaded from path
        /// </summary>
        IInputData LoadInputData(string inputFilePath);

        /// <summary>
        /// Saves result to path
        /// </summary>
        void SaveOutputData(string outputFilePath);
    }
}
