﻿using ISynergy.Framework.Mathematics.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISynergy.Framework.Mathematics.Tests
{
    [TestClass]
    public class SparseTest
    {
        [TestMethod]
        public void SetTest()
        {
            var s = new Sparse<double>();
            s[0] = 1;
            s[99] = 99;
            s[10] = 42;

            double[] d = s.ToDense();

            for (int i = 0; i < 100; i++)
            {
                if (i == 0)
                {
                    Assert.AreEqual(1, s[i]);
                    Assert.AreEqual(1, d[i]);
                }
                else if (i == 10)
                {
                    Assert.AreEqual(42, s[i]);
                    Assert.AreEqual(42, d[i]);
                }
                else if (i == 99)
                {
                    Assert.AreEqual(99, s[i]);
                    Assert.AreEqual(99, d[i]);
                }
                else
                {
                    Assert.AreEqual(0, s[i]);
                    Assert.AreEqual(0, d[i]);
                }
            }
        }

        [TestMethod]
        public void ToStringTest()
        {
            double[] v;
            string actual;
            Sparse<double> d;

            v = new double[] { 1, 2, 3, 0, 0, 6 };
            d = Sparse.FromDense(v);

            actual = d.ToString();
            Assert.AreEqual("1:1 2:2 3:3 6:6", actual);

            v = new double[] { 0, 0, 2, 3, 0, 0, 6 };
            d = Sparse.FromDense(v);

            actual = d.ToString();
            Assert.AreEqual("3:2 4:3 7:6", actual);
        }

        [TestMethod]
        public void ParseTest()
        {
            double[] v;
            Sparse<double> actual;
            Sparse<double> expected;
            string s;

            v = new double[] { 1, 2, 3, 0, 0, 6 };
            expected = Sparse.FromDense(v);
            s = expected.ToString();
            actual = Sparse.Parse(s);
            CollectionAssert.AreEqual(expected, actual);

            v = new double[] { 0, 2, 3, 0, 0, 6 };
            expected = Sparse.FromDense(v);
            s = expected.ToString();
            actual = Sparse.Parse(s);
            CollectionAssert.AreEqual(expected, actual);


            v = new double[] { 1, 2, 3, 0, 0, 6 };
            expected = Sparse.FromDense(v);
            s = expected.ToString();
            actual = Sparse.Parse(s, insertValueAtBeginning: 0);
            CollectionAssert.AreEqual(expected, actual);

            v = new double[] { 0, 2, 3, 0, 0, 6 };
            expected = Sparse.FromDense(v);
            s = expected.ToString();
            actual = Sparse.Parse(s, insertValueAtBeginning: 0);
            CollectionAssert.AreEqual(expected, actual);

            v = new double[] { 1, 2, 3, 0, 0, 6 };
            expected = Sparse.FromDense(v);
            s = expected.ToString();
            actual = Sparse.Parse(s, insertValueAtBeginning: 1);
            expected = Sparse.Parse("1:1 2:1 3:2 4:3 7:6");
            CollectionAssert.AreEqual(expected, actual);

            v = new double[] { 0, 2, 3, 0, 0, 6 };
            expected = Sparse.FromDense(v);
            s = expected.ToString();
            actual = Sparse.Parse(s, insertValueAtBeginning: 42);
            expected = Sparse.Parse("1:42 3:2 4:3 7:6");
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
