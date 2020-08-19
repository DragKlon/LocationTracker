using LocationTracker.Contracts.Interfaces;

namespace LocationTracker.Contracts
{
    /// <summary>
    /// Basis view with specific Tracker
    /// </summary>
    public interface IView : IValidatingView
    {
        /// <summary>
        /// Is used to create TrackerFactory
        /// </summary>
        ITrackerFactory TrackerFactory { get; }

        /// <summary>
        /// Gets the specific type of tracker
        /// </summary>
        ITracker Tracker { get; }
    }
}
