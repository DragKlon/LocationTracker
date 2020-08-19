using System;

namespace LocationTracker.Helpers
{
    /// <summary>
    /// Class to extend double type with Square method
    /// </summary>
    public static class DoubleSquareExtension
    {
        /// <summary>
        /// Returns squared double
        /// </summary>
        public static double Square(this double d)
        {
            return Math.Pow(d, 2);
        }
    }
}
