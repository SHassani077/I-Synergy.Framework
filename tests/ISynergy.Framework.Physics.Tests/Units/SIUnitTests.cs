﻿using ISynergy.Framework.Physics.Enumerations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ISynergy.Framework.Physics.Tests
{
    /// <summary>
    /// Test of BaseUnits.
    /// </summary>
    [TestClass()]
    public class SIUnitTests
    {
        /// <summary>
        /// If BaseUnit is created with empty parameters it should fail.
        /// </summary>
        [TestMethod()]
        public void BaseUnitEmptyContructorTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new SIUnit(Enumerations.Units.second, Array.Empty<UnitTypes>()));
        }

        /// <summary>
        /// If BaseUnit is created with nulled parameters it should fail.
        /// </summary>
        [TestMethod()]
        public void BaseUnitNulledConstructorTest()
        {
            Assert.ThrowsException<ArgumentOutOfRangeException>(() => new SIUnit(Enumerations.Units.second, Array.Empty<UnitTypes>()));
        }

        /// <summary>
        /// If unit is created successfully, then unit should not be null.
        /// </summary>
        [TestMethod()]
        public void BaseUnitValidConstructorTest()
        {
            var unit = new SIUnit(Enumerations.Units.second, new UnitTypes[] { UnitTypes.Time });
            Assert.IsNotNull(unit);
        }
    }
}