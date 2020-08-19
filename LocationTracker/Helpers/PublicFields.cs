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
            char configsPositionSeparator = ',';
            char.TryParse(ConfigurationManager.AppSettings["PositionSeparator"], out configsPositionSeparator);
            PositionSeparator = configsPositionSeparator;

            double configsWavesSpeed = 1000000;
            double.TryParse(ConfigurationManager.AppSettings["WavesSpeed"], out configsWavesSpeed);
            WavesSpeed = configsWavesSpeed;

            int configsError = 5;
            int.TryParse(ConfigurationManager.AppSettings["MeasurmentErrorPercentage"], out configsError);
            Error = configsError;
        }
    }
}
