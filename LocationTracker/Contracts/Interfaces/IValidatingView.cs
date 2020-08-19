namespace LocationTracker.Contracts.Interfaces
{
    /// <summary>
    /// Classifies type of Views which should validate some data
    /// </summary>
    public interface IValidatingView
    {
        /// <summary>
        /// Gets the specific validator
        /// </summary>
        IValidator Validator { get; }
    }
}
