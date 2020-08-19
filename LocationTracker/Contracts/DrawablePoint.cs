namespace LocationTracker.Contracts
{
    /// <summary>
    /// Classifies two dimensial points to draw at any graphic
    /// </summary>
    public class DrawablePoint
    {
        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="point">Point</param>
        /// <param name="x">Recalculated X position in pixels</param>
        /// <param name="y">Recalculated Y position in pixels</param>
        public DrawablePoint(TwoDimensialPoint point, double x, double y)
        {
            Point = point;
            RecalculatedX = x;
            RecalculatedY = y;
        }

        /// <summary>
        /// Two dimensial points to draw
        /// </summary>
        public TwoDimensialPoint Point { get; set; }

        /// <summary>
        /// Recalculated X position in pixels
        /// </summary>
        public double RecalculatedX { get; set; }

        /// <summary>
        /// Recalculated Y position in pixels
        /// </summary>
        public double RecalculatedY { get; set; }
    }
}
