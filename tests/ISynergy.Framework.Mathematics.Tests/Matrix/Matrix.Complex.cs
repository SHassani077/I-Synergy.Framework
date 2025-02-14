﻿namespace ISynergy.Framework.Mathematics.Tests
{
    using ISynergy.Framework.Mathematics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Numerics;

    [TestClass]
    public class ComplexMatrixTest
    {

        [TestMethod]
        public void AbsTest()
        {
            Complex[] x = { new Complex(1, 5), new Complex(2, -1), new Complex(-5, 1) };
            Complex[] expected = { new Complex(Math.Sqrt(26), 0), new Complex(Math.Sqrt(5), 0), new Complex(Math.Sqrt(26), 0) };
            Complex[] actual = ComplexMatrix.Abs(x);
            Assert.IsTrue(expected.IsEqual(actual, 1e-5));
        }

        [TestMethod]
        public void ImTest()
        {
            Complex[] x = { new Complex(1, 5), new Complex(2, -1), new Complex(-5, 1) };
            double[] expected = { 5, -1, 1 };
            double[] actual = ComplexMatrix.Im(x);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod]
        public void MagnitudeTest()
        {
            Complex[] x = { new Complex(1, 5), new Complex(2, -1), new Complex(-5, 1) };
            double[] expected = { Math.Sqrt(26), Math.Sqrt(5), Math.Sqrt(26) };
            double[] actual = ComplexMatrix.Magnitude(x);

            Assert.IsTrue(expected.IsEqual(actual, 1e-12));
        }

        [TestMethod]
        public void MultiplyTest()
        {
            Complex[] a = { new Complex(7, 5), new Complex(2, -3), new Complex(-5, 1) };
            Complex[] b = { new Complex(1, 5), new Complex(8, -1), new Complex(-4, 8) };
            Complex[] expected = { new Complex(-18, 40), new Complex(13, -26), new Complex(12, -44) };
            Complex[] actual = ComplexMatrix.Multiply(a, b);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod]
        public void PhaseTest()
        {
            Complex[] x = { new Complex(0, 5), new Complex(2, 0), new Complex(-5, 1) };
            double[] expected = { 1, Math.Sqrt(5), Math.Sqrt(26) };
            double[] actual = ComplexMatrix.Phase(x);

            for (int i = 0; i < x.Length; i++)
                Assert.AreEqual(x[i].Phase, Math.Atan2(x[i].Imaginary, x[i].Real));
        }


        [TestMethod]
        public void ReTest()
        {
            Complex[] x = { new Complex(1, 5), new Complex(2, -1), new Complex(-5, 1) };
            double[] expected = { 1, 2, -5 };
            double[] actual = ComplexMatrix.Re(x);

            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod]
        public void SumTest()
        {
            Complex[] x = { new Complex(1, 5), new Complex(2, -1), new Complex(-5, 1) };
            Complex expected = new Complex(-2, 5);
            Complex actual = ComplexMatrix.Sum(x);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void ToArrayTest()
        {
            Complex[] c = { new Complex(1, 5), new Complex(2, -1), new Complex(-5, 1) };
            double[,] expected = 
            {
                {  1, 5  },
                {  2, -1 },
                { -5, 1  },
            };

            double[,] actual = ComplexMatrix.ToArray(c);

            Assert.IsTrue(expected.IsEqual(actual));
        }


        [TestMethod]
        public void MatrixImTest()
        {
            Complex[,] x = 
            {
                { new Complex(1, 5), new Complex(2, 6) },
                { new Complex(3, 7), new Complex(4, 8) },
                { new Complex(5, 9), new Complex(6, 0) },
            };

            double[,] expected = 
            { 
                { 5, 6 },
                { 7, 8 },
                { 9, 0 },
            };

            double[,] actual = ComplexMatrix.Im(x);
            Assert.IsTrue(expected.IsEqual(actual));
        }

        [TestMethod]
        public void MatrixReTest()
        {
            Complex[,] x = 
            {
                { new Complex(1, 5), new Complex(2, 6) },
                { new Complex(3, 7), new Complex(4, 8) },
                { new Complex(5, 9), new Complex(6, 0) },
            };

            double[,] expected = 
            { 
                { 1, 2 },
                { 3, 4 },
                { 5, 6 },
            };

            double[,] actual = ComplexMatrix.Re(x);
            Assert.IsTrue(expected.IsEqual(actual));
        }
    }
}
