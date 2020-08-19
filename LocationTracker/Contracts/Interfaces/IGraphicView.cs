namespace LocationTracker.Contracts
{
    /// <summary>
    /// Classifies types of Views which will be presented with some graphic
    /// </summary>
    public interface IGraphicView
    {
        /// <summary>
        /// Draws short cutoff lines at the graphic's axis
        /// </summary>
        void DrawFixedValuesLines();
    }
}
