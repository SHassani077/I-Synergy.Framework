﻿using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class QueryableTests.
    /// </summary>
    public partial class QueryableTests
    {
        /// <summary>
        /// Defines the test method Average.
        /// </summary>
        [TestMethod]
        public void Average()
        {
            // Arrange
            var incomes = User.GenerateSampleModels(100).Select(u => u.Income);

            // Act
            var expected = incomes.Average();
            var actual = incomes.AsQueryable().Average();

            // Assert
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Defines the test method Average_Selector.
        /// </summary>
        [TestMethod]
        public void Average_Selector()
        {
            // Arrange
            var users = User.GenerateSampleModels(100);

            // Act
            var expected = users.Average(u => u.Income);
            var result = users.AsQueryable().Average("Income");

            // Assert
            Assert.AreEqual(expected, result);
        }
    }
}
