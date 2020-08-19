namespace LocationTracker.Contracts
{
    /// <summary>
    /// Classifies location validator
    /// </summary>
    public interface ILocationValidator
    {
        /// <summary>
        /// Validates does location is presented correctly for the tracker type
        /// </summary>
        /// <param name="location">Location presented as string</param>
        /// <param name="trackerType">TrackerTypes.TwoDimensial be deafault</param>
        bool ValidateLocation(string location, TrackerTypes trackerType = TrackerTypes.TwoDimensial);
    }
}
