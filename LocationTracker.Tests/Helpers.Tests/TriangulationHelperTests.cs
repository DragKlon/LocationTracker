using System;
using System.Linq;
using LocationTracker.Contracts;
using LocationTracker.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LocationTracker.Tests
{
    [TestClass]
    public class TriangulationHelperTests : TriangulationHelper
    {
        [TestMethod]
        public void TwoCirclesIntersection_TwoIntersection_ReturnsTwoDifferentPoints()
        {
            TwoDimensialPoint firstCenter = new TwoDimensialPoint { XPosition = 1, YPosition = 2 };
            TwoDimensialPoint secondCenter = new TwoDimensialPoint { XPosition = 5, YPosition = 5 };
            double firstRadius = 3;
            double secondRadius = 4;

            var intersections = TwoCirclesIntersection(firstCenter, secondCenter, firstRadius, secondRadius);

            Assert.IsTrue(intersections.Count() == 2);
            Assert.IsTrue(intersections.Any(i => i.XPosition == 3.88) && intersections.Any(i => i.XPosition == 1));
            Assert.IsTrue(intersections.Any(i => i.YPosition == 1.16000000) && intersections.Any(i => i.YPosition == 5));
        }

        [TestMethod]
        public void TwoCirclesIntersection_OneIntersection_ReturnsTwoSamePositionsPoints()
        {
            TwoDimensialPoint firstCenter = new TwoDimensialPoint { XPosition = 1, YPosition = 2 };
            TwoDimensialPoint secondCenter = new TwoDimensialPoint { XPosition = 6, YPosition = 2 };
            double firstRadius = 3;
            double secondRadius = 2;

            var intersections = TwoCirclesIntersection(firstCenter, secondCenter, firstRadius, secondRadius);

            Assert.IsTrue(intersections.Count() == 2);
            Assert.IsTrue(intersections.All(i => i.XPosition == 4));
            Assert.IsTrue(intersections.All(i => i.YPosition == 2));
        }

        [TestMethod]
        public void TwoCirclesIntersection_WithoutIntersections_ReturnsEmptyCollection()
        {
            TwoDimensialPoint firstCenter = new TwoDimensialPoint { XPosition = 1, YPosition = 2 };
            TwoDimensialPoint secondCenter = new TwoDimensialPoint { XPosition = 6, YPosition = 3 };
            double firstRadius = 3;
            double secondRadius = 2;

            var intersections = TwoCirclesIntersection(firstCenter, secondCenter, firstRadius, secondRadius);

            Assert.IsTrue(intersections.Count() == 0);
        }
    }
}
