using System.Text;

namespace LocationTracker.Helpers
{
    /// <summary>
    /// Class to extend StringBuilder type with Trim method
    /// </summary>
    public static class StringBuilderTrim
    {
        /// <summary>
        /// Removes specific trimChar from start and end of the stringBuilder
        /// </summary>
        /// <returns>New StringBuilder</returns>
        public static StringBuilder Trim(this StringBuilder stringBuilder, char trimChar)
        {
            var resultString = stringBuilder.ToString().Trim(trimChar);
            return new StringBuilder(resultString);
        }
    }
}
