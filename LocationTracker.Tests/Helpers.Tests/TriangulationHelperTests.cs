using System;
using System.Collections.Generic;
using System.Linq;
using LocationTracker.Contracts;
using LocationTracker.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace LocationTracker.Tests
{
    [TestClass]
    public class TriangulationHelperTests : TriangulationHelper
    {
        Mock<TriangulationHelperTests> subject;

        [TestInitialize]
        public void Initialise()
        {
            subject = new Mock<TriangulationHelperTests> { CallBase = true };
        }

        [TestMethod]
        public void TwoCirclesIntersection_TwoIntersection_ReturnsTwoDifferentPoints()
        {
            TwoDimensialPoint firstCenter = new TwoDimensialPoint { XPosition = 1, YPosition = 2 };
            TwoDimensialPoint secondCenter = new TwoDimensialPoint { XPosition = 5, YPosition = 5 };
            double firstRadius = 3;
            double secondRadius = 4;

            var intersections = subject.Object.TwoCirclesIntersection(firstCenter, secondCenter, firstRadius, secondRadius);

            //Expected values were calculated by online circles intersection calculator
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

            var intersections = subject.Object.TwoCirclesIntersection(firstCenter, secondCenter, firstRadius, secondRadius);

            //Expected values were calculated by online circles intersection calculator
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

            var intersections = subject.Object.TwoCirclesIntersection(firstCenter, secondCenter, firstRadius, secondRadius);

            //Expected values were calculated by online circles intersection calculator
            Assert.IsTrue(intersections.Count() == 0);
        }

        [TestMethod]
        public void GetPosition_TwoDimensialReceivers_ReturnsGetTwoDimensialPositionResult()
        {
            var getTwoDimensialPositionResultPoint = new TwoDimensialPoint();
            var receivers = new List<TwoDimensialPoint> { new TwoDimensialPoint(), new TwoDimensialPoint(), new TwoDimensialPoint() };
            var propogationTimes = new List<double>();
            subject.Setup(m => m.GetTwoDimensialPosition(receivers, propogationTimes, 0)).Returns(getTwoDimensialPositionResultPoint);

            var result = subject.Object.GetPosition(receivers, propogationTimes);

            Assert.AreEqual(getTwoDimensialPositionResultPoint, result);
        }

        [TestMethod]
        public void GetPosition_ThreeDimensialReceivers_ReturnsNull()
        {
            var receivers = new List<IPoint> { new ThreeDimensialPoint(), new ThreeDimensialPoint(), new ThreeDimensialPoint() };
            var propogationTimes = new List<double>();

            var result = subject.Object.GetPosition(receivers, propogationTimes);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TwoCirclesIntersection_WithErrorHandlingIntersectionsExists_ReturnsPointsCollection()
        {
            double firstRadius = 3;
            double secondRadius = 4;
            int error = 5;
            var firstCenter = new TwoDimensialPoint { XPosition = 1, YPosition = 2 };
            var secondCenter = new TwoDimensialPoint { XPosition = 4, YPosition = 8 };
            var intersections = new List<TwoDimensialPoint> { new TwoDimensialPoint(), new TwoDimensialPoint() };
            double r1Minimum = firstRadius - firstRadius * error / 100;
            double r2Minimum = secondRadius - secondRadius * error / 100;
            subject.Setup(m => m.TwoCirclesIntersection(firstCenter, secondCenter, r1Minimum, r2Minimum))
                .Returns(intersections);

            var result = subject.Object.TwoCirclesIntersection(firstCenter, secondCenter, firstRadius, secondRadius, error);

            Assert.AreEqual(intersections, result);
        }

        [TestMethod]
        public void TwoCirclesIntersection_NoIntersectionsWithErrorHandling_ReturnsEmptyCollection()
        {
            double firstRadius = 3;
            double secondRadius = 4;
            int error = 5;
            var firstCenter = new TwoDimensialPoint { XPosition = 1, YPosition = 2 };
            var secondCenter = new TwoDimensialPoint { XPosition = 4, YPosition = 8 };
            double r1Minimum = firstRadius - firstRadius * error / 100;
            double r1Maximum = firstRadius + firstRadius * error / 100;
            double r2Minimum = secondRadius - secondRadius * error / 100;
            double r2Maximum = secondRadius + secondRadius * error / 100;
            subject.Setup(m => m.TwoCirclesIntersection(firstCenter, secondCenter, r1Minimum, r2Minimum))
                .Returns(new List<TwoDimensialPoint>());
            subject.Setup(m => m.TwoCirclesIntersection(firstCenter, secondCenter, r1Minimum, r2Maximum))
                .Returns(new List<TwoDimensialPoint>());
            subject.Setup(m => m.TwoCirclesIntersection(firstCenter, secondCenter, r1Maximum, r2Minimum))
                .Returns(new List<TwoDimensialPoint>());
            subject.Setup(m => m.TwoCirclesIntersection(firstCenter, secondCenter, r1Maximum, r2Maximum))
                .Returns(new List<TwoDimensialPoint>());

            var result = subject.Object.TwoCirclesIntersection(firstCenter, secondCenter, firstRadius, secondRadius, error);

            Assert.IsTrue(result.Count() == 0);
        }
    }
}
