﻿using System;
using ISynergy.Framework.Geography.Common;
using ISynergy.Framework.Geography.Global;
using ISynergy.Framework.Geography.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Geography.Geodetic.Tests
{
    /// <summary>
    /// Class GeodeticCalculatorTests.
    /// </summary>
    [TestClass]
    public class GeodeticCalculatorTests
    {
        /// <summary>
        /// The calculate
        /// </summary>
        private readonly GeodeticCalculator calc = new GeodeticCalculator(Ellipsoid.WGS84);

        /// <summary>
        /// Defines the test method TestCurve.
        /// </summary>
        [TestMethod]
        public void TestCurve()
        {
            var curve = calc.CalculateGeodeticCurve(Constants.MyHome, Constants.MyOffice);
            // Reference values computed with 
            // http://williams.best.vwh.net/gccalc.htm
            Assert.AreEqual(43232.317, Math.Round(1000 * curve.EllipsoidalDistance) / 1000);
            Assert.AreEqual(342.302315, Math.Round(1000000 * curve.Azimuth.Degrees) / 1000000);
            Assert.AreEqual(curve.Calculator, calc);
        }

        /// <summary>
        /// Defines the test method TestCurve2.
        /// </summary>
        [TestMethod]
        public void TestCurve2()
        {
            var target = new GlobalCoordinates(Constants.MyHome.Latitude, Constants.MyHome.Longitude - 1.0);
            var curve = calc.CalculateGeodeticCurve(Constants.MyHome, target);
            // Reference values computed with 
            // http://williams.best.vwh.net/gccalc.htm
            Assert.AreEqual(270.382160, Math.Round(100000 * curve.Azimuth.Degrees) / 100000);
            Assert.AreEqual(curve.Calculator, calc);
        }

        /// <summary>
        /// Defines the test method TestMeasurement.
        /// </summary>
        [TestMethod]
        public void TestMeasurement()
        {
            var start = new GlobalPosition(Constants.MyHome, 200);
            var end = new GlobalPosition(Constants.MyOffice, 240);
            var m = calc.CalculateGeodeticMeasurement(start, end);
            var c = calc.CalculateGeodeticCurve(Constants.MyHome, Constants.MyOffice);
            Assert.IsTrue(m.EllipsoidalDistance > c.EllipsoidalDistance);
        }

        /// <summary>
        /// Defines the test method TestNearAntipodicCurve.
        /// </summary>
        [TestMethod]
        public void TestNearAntipodicCurve()
        {
            var loc = new GlobalCoordinates(0, 10); // on the equator
            var aloc = loc.Antipode;
            aloc.Latitude *= 0.99999998;
            var curve = calc.CalculateGeodeticCurve(loc, aloc);
            Assert.IsTrue(double.IsNaN(curve.Azimuth.Degrees));
            Assert.AreEqual(curve.Calculator, calc);
        }

        /// <summary>
        /// Defines the test method TestEnding.
        /// </summary>
        [TestMethod]
        public void TestEnding()
        {
            var curve = calc.CalculateGeodeticCurve(Constants.MyHome, Constants.MyOffice);
            var final = calc.CalculateEndingGlobalCoordinates(
                Constants.MyHome,
                curve.Azimuth,
                curve.EllipsoidalDistance);
            Assert.AreEqual(final, Constants.MyOffice);
            Assert.AreEqual(curve.Calculator, calc);
        }

        /// <summary>
        /// Defines the test method TestEndingNegDist.
        /// </summary>
        [TestMethod]
        public void TestEndingNegDist()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => calc.CalculateEndingGlobalCoordinates(
                Constants.MyHome,
                0.0,
                -1.0));
        }

        /// <summary>
        /// Defines the test method TestEndingZeroDist.
        /// </summary>
        [TestMethod]
        public void TestEndingZeroDist()
        {
            var curve = calc.CalculateGeodeticCurve(Constants.MyHome, Constants.MyOffice);
            var final = calc.CalculateEndingGlobalCoordinates(
                Constants.MyHome,
                curve.Azimuth,
                0.0);
            Assert.AreEqual(final, Constants.MyHome);
            Assert.AreEqual(curve.Calculator, calc);
        }

        /// <summary>
        /// Defines the test method TestPath1.
        /// </summary>
        [TestMethod]
        public void TestPath1()
        {
            var path = calc.CalculateGeodeticPath(Constants.MyHome, Constants.MyOffice, 2);
            Assert.AreEqual(path[0], Constants.MyHome);
            Assert.AreEqual(path[1], Constants.MyOffice);
            Assert.AreEqual(calc.ReferenceGlobe, Ellipsoid.WGS84);
        }

        /// <summary>
        /// Defines the test method TestPath2.
        /// </summary>
        [TestMethod]
        public void TestPath2()
        {
            var path = calc.CalculateGeodeticPath(Constants.MyHome, Constants.MyOffice);
            Assert.AreEqual(path[0], Constants.MyHome);
            Assert.AreEqual(path[path.Length - 1], Constants.MyOffice);
            Assert.AreEqual(calc.ReferenceGlobe, Ellipsoid.WGS84);
        }

        /// <summary>
        /// Defines the test method TestPath3.
        /// </summary>
        [TestMethod]
        public void TestPath3()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => calc.CalculateGeodeticPath(Constants.MyHome, Constants.MyOffice, 1));
        }

        /// <summary>
        /// Defines the test method TestPath4.
        /// </summary>
        [TestMethod]
        public void TestPath4()
        {
            var path = calc.CalculateGeodeticPath(Constants.MyHome, Constants.MyHome, 10);
            Assert.AreEqual(2, path.Length);
            Assert.AreEqual(path[0], Constants.MyHome);
            Assert.AreEqual(path[1], Constants.MyHome);
        }
    }
}
