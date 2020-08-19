using LocationTracker.Contracts;
using LocationTracker.Validators;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationTracker.Tests
{
    [TestClass]
    public class DefaultValidatorTests : DefaultValidator
    {
        Mock<DefaultValidatorTests> subject;

        [TestInitialize]
        public void MyTestMethod()
        {
            subject = new Mock<DefaultValidatorTests>() { CallBase = true };
        }

        [TestMethod]
        public void ValidateInputFile_WithoutTrackerTypeDefinition_ReturnsTwoDimensialTrackerValidationResult()
        {
            bool twoDimensialValidationResult = true;
            string inputPath = "Input.path";
            subject.Setup(m => m.ValidateTwoDimensialInputFile(inputPath)).Returns(twoDimensialValidationResult);

            var result = subject.Object.ValidateInputFile(inputPath);

            Assert.AreEqual(twoDimensialValidationResult, result);
        }

        [TestMethod]
        public void ValidateInputFile_WithThreeDimensialTrackerType_ReturnsFalse()
        {
            string inputPath = "Input.path";

            var result = subject.Object.ValidateInputFile(inputPath, TrackerTypes.ThreeDimensial);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateTwoDimensialInputFile_InputFileWithOneLine_ReturnsFalse()
        {
            string inputPath = "Input.path";
            subject.Setup(m => m.ReadAllLines(inputPath)).Returns(new string[] { "first line" });

            var result = subject.Object.ValidateTwoDimensialInputFile(inputPath);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateTwoDimensialInputFile_InputFileWithIncorrectReceivers_ReturnsFalse()
        {
            string inputPath = "Input.path";
            string receivers = "receivers";
            string[] inputData = new string[] { receivers, "first line", "second line" };
            subject.Setup(m => m.ReadAllLines(inputPath)).Returns(inputData);
            subject.Setup(m => m.ValidateTwoDimensialReceivers(receivers)).Returns(false);

            var result = subject.Object.ValidateTwoDimensialInputFile(inputPath);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateTwoDimensialInputFile_InputFileWithIncorrectLocations_ReturnsFalse()
        {
            string inputPath = "Input.path";
            string receivers = "receivers";
            string firstLineData = "first line";
            string[] inputData = new string[] { receivers, firstLineData };
            subject.Setup(m => m.ReadAllLines(inputPath)).Returns(inputData);
            subject.Setup(m => m.ValidateTwoDimensialReceivers(receivers)).Returns(true);
            subject.Setup(m => m.ValidatePropagationTimesLine(firstLineData)).Returns(false);

            var result = subject.Object.ValidateTwoDimensialInputFile(inputPath);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateTwoDimensialInputFile_InputFileWithCorrectData_ReturnsTrue()
        {
            string inputPath = "Input.path";
            string receivers = "receivers";
            string firstLineData = "first line";
            string[] inputData = new string[] { receivers, firstLineData };
            subject.Setup(m => m.ReadAllLines(inputPath)).Returns(inputData);
            subject.Setup(m => m.ValidateTwoDimensialReceivers(receivers)).Returns(true);
            subject.Setup(m => m.ValidatePropagationTimesLine(firstLineData)).Returns(true);

            var result = subject.Object.ValidateTwoDimensialInputFile(inputPath);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidatePropagationTimesLine_ValidData_ReturnsTrue()
        {
            string propogationTimeLine = $"0.001{PublicFields.PositionSeparator}0.002{PublicFields.PositionSeparator}0.003";

            var result = subject.Object.ValidatePropagationTimesLine(propogationTimeLine);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidatePropagationTimesLine_LineWithInvalidCulture_ReturnsFalse()
        {
            string propogationTimeLine = $"0,001{PublicFields.PositionSeparator}0,002{PublicFields.PositionSeparator}0,003";

            var result = subject.Object.ValidatePropagationTimesLine(propogationTimeLine);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidatePropagationTimesLine_LineWithInvalidContent_ReturnsFalse()
        {
            string propogationTimeLine = "invalid data!";

            var result = subject.Object.ValidatePropagationTimesLine(propogationTimeLine);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidatePropagationTimesLine_LineWithTwoTimes_ReturnsFalse()
        {
            string propogationTimeLine = $"0.001{PublicFields.PositionSeparator}0.002";

            var result = subject.Object.ValidatePropagationTimesLine(propogationTimeLine);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateTwoDimensialReceivers_ValidReceviers_ReturnsTrue()
        {
            string receivers = $"0.001{PublicFields.PositionSeparator}0.002{PublicFields.PositionSeparator}0.003" +
                $"{PublicFields.PositionSeparator}0.004{PublicFields.PositionSeparator}0.005{PublicFields.PositionSeparator}0.006";

            var result = subject.Object.ValidateTwoDimensialReceivers(receivers);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateTwoDimensialReceivers_InvalidReceviersNumber_ReturnsFalse()
        {
            string receivers = $"0.001{PublicFields.PositionSeparator}0.002{PublicFields.PositionSeparator}0.003" +
                $"{PublicFields.PositionSeparator}0.004";

            var result = subject.Object.ValidateTwoDimensialReceivers(receivers);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateTwoDimensialReceivers_InvalidCulture_ReturnsFalse()
        {
            string receivers = $"0,001{PublicFields.PositionSeparator}0,002{PublicFields.PositionSeparator}0,003" +
                $"{PublicFields.PositionSeparator}0,004{PublicFields.PositionSeparator}0.005{PublicFields.PositionSeparator}0.006";

            var result = subject.Object.ValidateTwoDimensialReceivers(receivers);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateTwoDimensialReceivers_InvalidData_ReturnsFalse()
        {
            string receivers = "invalid data";

            var result = subject.Object.ValidateTwoDimensialReceivers(receivers);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateLocation_ForTwoDimensialTracker_ReturnsValidateTwoDimensialLocationResult()
        {
            bool twoDimensialValidationResult = true;
            string location = "location";
            subject.Setup(m => m.ValidateTwoDimensialLocation(location)).Returns(twoDimensialValidationResult);

            var result = subject.Object.ValidateInputFile(location);

            Assert.AreEqual(twoDimensialValidationResult, result);
        }

        [TestMethod]
        public void ValidateLocation_ForThreeDimensialTracker_ReturnsFalse()
        {
            string location = "location";

            var result = subject.Object.ValidateInputFile(location, TrackerTypes.ThreeDimensial);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateTwoDimensialLocation_ValidLocation_ReturnsTrue()
        {
            string location = $"0.001{PublicFields.PositionSeparator}0.002";

            var result = subject.Object.ValidateTwoDimensialLocation(location);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidateTwoDimensialLocation_InvalidLocation_ReturnsFalse()
        {
            string location = $"invalid location";

            var result = subject.Object.ValidateTwoDimensialLocation(location);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateTwoDimensialLocation_LocationWithInvalidCulture_ReturnsFalse()
        {
            string location = $"0,001{PublicFields.PositionSeparator}0,002";

            var result = subject.Object.ValidateTwoDimensialLocation(location);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidateTwoDimensialLocation_LocationWithInvalidPositionNumbers_ReturnsFalse()
        {
            string location = $"0.001{PublicFields.PositionSeparator}0.002{PublicFields.PositionSeparator}0.003";

            var result = subject.Object.ValidateTwoDimensialLocation(location);

            Assert.IsFalse(result);
        }

    }
}
