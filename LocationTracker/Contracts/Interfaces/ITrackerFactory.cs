namespace LocationTracker.Contracts
{
    /// <summary>
    /// Classifies factory to create a tracker
    /// </summary>
    public interface ITrackerFactory
    {
        /// <summary>
        /// Returns a Tracker with the specified type
        /// </summary>
        /// <param name="tracker">Tracker type to create</param>
        ITracker CreateFactory(TrackerTypes tracker = TrackerTypes.TwoDimensial);
    }
}
