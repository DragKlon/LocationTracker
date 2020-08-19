using System.Collections.Generic;

namespace LocationTracker.Contracts
{
    /// <summary>
    /// Classifies a specific input data including receivers and propogation times
    /// </summary>
    public class InputData : IInputData
    {
        /// <summary>
        /// Receivers locations collection
        /// </summary>
        internal IEnumerable<IPoint> Receivers { get; set; }

        /// <summary>
        /// Signal propagation to receivers time
        /// </summary>
        internal IEnumerable<IEnumerable<double>> PropagationTime { get; set; }
    }
}
