namespace LocationTracker.Contracts
{
    /// <summary>
    /// Classifies paths validator
    /// </summary>
    public interface IPathsValidator
    {
        /// <summary>
        /// Checks if input data has correct content
        /// </summary>
        /// <param name="inputPath">Input data file path</param>
        /// <param name="trackerType">Which tracker type will work with data</param>
        bool ValidateInputFile(string inputPath, TrackerTypes trackerType = TrackerTypes.TwoDimensial);

        /// <summary>
        /// Checks does input file exists
        /// </summary>
        /// <param name="path">Input file path</param>
        bool ValidateInputPath(string path);

        /// <summary>
        /// Checks does any file already exists for the path
        /// </summary>
        /// <param name="path">Output file path</param>
        bool ValidateOutputPath(string path);
    }
}
