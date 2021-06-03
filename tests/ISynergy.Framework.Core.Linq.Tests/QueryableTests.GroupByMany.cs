﻿using System;
using System.Collections.Generic;
using System.Linq;
using NFluent;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Core.Linq.Extensions.Tests
{
    /// <summary>
    /// Class QueryableTests.
    /// </summary>
    public partial class QueryableTests
    {
        /// <summary>
        /// Defines the test method GroupByMany_Dynamic_LambdaExpressions.
        /// </summary>
        [TestMethod]
        public void GroupByMany_Dynamic_LambdaExpressions()
        {
            var lst = new List<Tuple<int, int, int>>
            {
                new Tuple<int, int, int>(1, 1, 1),
                new Tuple<int, int, int>(1, 1, 2),
                new Tuple<int, int, int>(1, 1, 3),
                new Tuple<int, int, int>(2, 2, 4),
                new Tuple<int, int, int>(2, 2, 5),
                new Tuple<int, int, int>(2, 2, 6),
                new Tuple<int, int, int>(2, 3, 7)
            };

            var sel = lst.AsQueryable().GroupByMany(x => x.Item1, x => x.Item2);

            Assert.AreEqual(2, sel.Count());
            Assert.IsTrue(sel.First().Subgroups.Count() == 1);
            Assert.AreEqual(2, sel.Skip(1).First().Subgroups.Count());
        }

        /// <summary>
        /// Defines the test method GroupByMany_Dynamic_StringExpressions.
        /// </summary>
        [TestMethod]
        public void GroupByMany_Dynamic_StringExpressions()
        {
            var lst = new List<Tuple<int, int, int>>
            {
                new Tuple<int, int, int>(1, 1, 1),
                new Tuple<int, int, int>(1, 1, 2),
                new Tuple<int, int, int>(1, 1, 3),
                new Tuple<int, int, int>(2, 2, 4),
                new Tuple<int, int, int>(2, 2, 5),
                new Tuple<int, int, int>(2, 2, 6),
                new Tuple<int, int, int>(2, 3, 7)
            };

            var sel = lst.AsQueryable().GroupByMany("Item1", "Item2").ToList();

            Check.That(sel.Count).Equals(2);

            var firstGroupResult = sel.First();
            Check.That(firstGroupResult.ToString()).Equals("1 (3)");
            Check.That(firstGroupResult.Subgroups.Count()).Equals(1);

            var skippedGroupResult = sel.Skip(1).First();
            Check.That(skippedGroupResult.ToString()).Equals("2 (4)");
            Check.That(skippedGroupResult.Subgroups.Count()).Equals(2);
        }
    }
}
