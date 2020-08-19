using LocationTracker.Trackers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationTracker.Contracts
{
    /// <summary>
    /// Factory to create specific Tracker 
    /// </summary>
    class TrackerFactory : ITrackerFactory
    {
        /// <summary>
        /// Returns a Tracker with the specified type
        /// </summary>
        /// <param name="tracker">Tracker type to create</param>
        public ITracker CreateFactory(TrackerTypes tracker = TrackerTypes.TwoDimensial)
        {
            switch (tracker)
            {
                case TrackerTypes.TwoDimensial:
                default:
                    return new TwoDimensialTracker();
            }
        }
    }
}
