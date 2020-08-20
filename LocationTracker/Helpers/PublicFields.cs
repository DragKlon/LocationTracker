using System.Configuration;

namespace LocationTracker
{
    /// <summary>
    /// Public fields configured by App.config file
    /// </summary>
    public static class PublicFields
    {
        /// <summary>
        /// Indicates how different dimensions positions are separated
        /// </summary>
        public static char PositionSeparator { get; }

        /// <summary>
        /// Indicates wave propagation speed in meters per second
        /// </summary>
        public static double WavesSpeed { get; }

        /// <summary>
        /// Indicates measurment error in percentage
        /// </summary>
        public static int Error { get; }

        /// <summary>
        /// ctor
        /// </summary>
        static PublicFields()
        {
            // Public fields initialization from App.config file
            char.TryParse(ConfigurationManager.AppSettings["PositionSeparator"], out char configsPositionSeparator);
            PositionSeparator = configsPositionSeparator != 0 ? configsPositionSeparator : ',';

            double.TryParse(ConfigurationManager.AppSettings["WavesSpeed"], out double configsWavesSpeed);
            WavesSpeed = configsWavesSpeed != 0? configsWavesSpeed : 1000000;

            int.TryParse(ConfigurationManager.AppSettings["MeasurmentErrorPercentage"], out int configsError);
            Error = configsError != 0 ? configsError : 5;
        }
    }
}
