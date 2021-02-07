﻿using System.Linq;
using ISynergy.Framework.Core.Linq.Extensions.Tests.Helpers.Models;
using Xunit;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class QueryableTests.
    /// </summary>
    public partial class QueryableTests
    {
        /// <summary>
        /// Defines the test method Sum.
        /// </summary>
        [Fact]
        public void Sum()
        {
            // Arrange
            var incomes = User.GenerateSampleModels(100).Select(u => u.Income);

            // Act
            var expected = incomes.Sum();
            var actual = incomes.AsQueryable().Sum();

            // Assert
            Assert.Equal(expected, actual);
        }

        /// <summary>
        /// Defines the test method Sum_Selector.
        /// </summary>
        [Fact]
        public void Sum_Selector()
        {
            // Arrange
            var users = User.GenerateSampleModels(100);

            // Act
            var expected = users.Sum(u => u.Income);
            var result = users.AsQueryable().Sum("Income");

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
