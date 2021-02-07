﻿using System;
using ISynergy.Framework.Core.Data.Tests.TestClasses;
using Xunit;

namespace ISynergy.Framework.Core.Data.Tests
{
    /// <summary>
    /// Class ObservableClassTests.
    /// </summary>
    public class ObservableClassTests
    {
        // Check when object is initialized that it's clean.
        /// <summary>
        /// Defines the test method CheckIfObjectAfterInitializationIsClean_1.
        /// </summary>
        [Fact]
        public void CheckIfObjectAfterInitializationIsClean_1()
        {
            var product = new Product
            {
                Name = "Test1"
            };
            product.MarkAsClean();

            Assert.False(product.IsDirty);
        }

        /// <summary>
        /// Defines the test method CheckIfObjectAfterInitializationIsClean_2.
        /// </summary>
        [Fact]
        public void CheckIfObjectAfterInitializationIsClean_2()
        {
            var product = new Product
            {
                Name = "Test2"
            };
            product.MarkAsClean();

            Assert.False(product.IsDirty);
        }

        /// <summary>
        /// Defines the test method CheckIfObjectAfterInitializationIsClean_3.
        /// </summary>
        [Fact]
        public void CheckIfObjectAfterInitializationIsClean_3()
        {
            var product = new Product(
                Guid.NewGuid(),
                "Test3",
                1,
                100);

            Assert.False(product.IsDirty);
        }

        // Check when object is initialized that it's not dirty
        /// <summary>
        /// Defines the test method CheckIfObjectAfterInitializationIsDirty_1.
        /// </summary>
        [Fact]
        public void CheckIfObjectAfterInitializationIsDirty_1()
        {
            var product = new Product
            {
                Name = "Test1"
            };

            Assert.True(product.IsDirty);
        }

        /// <summary>
        /// Defines the test method CheckIfObjectAfterInitializationIsDirty_2.
        /// </summary>
        [Fact]
        public void CheckIfObjectAfterInitializationIsDirty_2()
        {
            var product = new Product { Name = "Test2" };
            product.Date = DateTimeOffset.Now;

            Assert.True(product.IsDirty);
        }

        /// <summary>
        /// Defines the test method CheckIfObjectAfterInitializationIsDirty_3.
        /// </summary>
        [Fact]
        public void CheckIfObjectAfterInitializationIsDirty_3()
        {
            var product = new Product(
                Guid.NewGuid(),
                "Test3",
                1,
                100)
            {
                Date = null
            };

            Assert.True(product.IsDirty);
        }

        // Check when object is initialized that all properties are added to Property dictionary
    }
}
