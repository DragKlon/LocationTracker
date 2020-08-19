using System.Collections;
using System.Collections.Generic;

namespace LocationTracker.Contracts
{
    /// <summary>
    /// Classifies Point's entity
    /// </summary>
    public interface IPoint
    {
        /// <summary>
        /// Collection of different dimension positions
        /// </summary>
        IEnumerable<double> Positions { get; set; }

        /// <summary>
        /// Gets or sets point's location as string
        /// </summary>
        string Location { get; set; }
    }
}
