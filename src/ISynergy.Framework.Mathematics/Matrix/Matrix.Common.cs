﻿using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mathematics.Common;
using ISynergy.Framework.Mathematics.Comparers;
using ISynergy.Framework.Mathematics.Decompositions;
using ISynergy.Framework.Mathematics.Enumerations;
using ISynergy.Framework.Mathematics.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ISynergy.Framework.Mathematics
{
    /// <summary>
    ///     Special matrix types.
    /// </summary>
    /// <seealso cref="VectorType" />
    [Flags]
    public enum MatrixType
    {
        /// <summary>
        ///     Symmetric matrix.
        /// </summary>
        Symmetric = 0,

        /// <summary>
        ///     Lower (left) triangular matrix.
        /// </summary>
        LowerTriangular = 1,

        /// <summary>
        ///     Upper (right) triangular matrix.
        /// </summary>
        UpperTriangular = 2,

        /// <summary>
        ///     Diagonal matrix.
        /// </summary>
        Diagonal = 4,

        /// <summary>
        ///     Rectangular matrix.
        /// </summary>
        Rectangular = 8,

        /// <summary>
        ///     Square matrix.
        /// </summary>
        Square = 16
    }

    public static partial class Matrix
    {
        /// <summary>
        ///     Determines whether the specified type is a jagged array.
        /// </summary>
        public static bool IsJagged(this Type type)
        {
            return type.IsArray && type.GetElementType().IsArray;
        }

        /// <summary>
        ///     Gets the type of the element in a jagged or multi-dimensional matrix.
        /// </summary>
        /// <param name="array">The array whose element type should be computed.</param>
        public static Type GetInnerMostType(this Array array)
        {
            return array.GetType().GetInnerMostType();
        }

        /// <summary>
        ///     Gets the number of bytes contained in an array.
        /// </summary>
        public static int GetNumberOfBytes(this Array array)
        {
            var elementType = array.GetInnerMostType();
            var elementSize = Marshal.SizeOf(elementType);
            var numberOfElements = array.GetTotalLength();
            return elementSize * numberOfElements;
        }

        /// <summary>
        ///     Convolves an array with the given kernel.
        /// </summary>
        /// <param name="a">A floating number array.</param>
        /// <param name="kernel">A convolution kernel.</param>
        public static double[] Convolve(this double[] a, double[] kernel)
        {
            return Convolve(a, kernel, false);
        }

        /// <summary>
        ///     Convolves an array with the given kernel.
        /// </summary>
        /// <param name="a">A floating number array.</param>
        /// <param name="kernel">A convolution kernel.</param>
        /// <param name="trim">
        ///     If <c>true</c> the resulting array will be trimmed to
        ///     have the same length as the input array. Default is false.
        /// </param>
        public static double[] Convolve(this double[] a, double[] kernel, bool trim)
        {
            double[] result;
            var m = (int)Math.Ceiling(kernel.Length / 2.0);

            if (trim)
            {
                result = new double[a.Length];

                for (var i = 0; i < result.Length; i++)
                {
                    result[i] = 0;
                    for (var j = 0; j < kernel.Length; j++)
                    {
                        var k = i - j + m - 1;
                        if (k >= 0 && k < a.Length)
                            result[i] += a[k] * kernel[j];
                    }
                }
            }
            else
            {
                result = new double[a.Length + m];

                for (var i = 0; i < result.Length; i++)
                {
                    result[i] = 0;
                    for (var j = 0; j < kernel.Length; j++)
                    {
                        var k = i - j;
                        if (k >= 0 && k < a.Length)
                            result[i] += a[k] * kernel[j];
                    }
                }
            }

            return result;
        }
        /// <summary>
        ///     Creates a memberwise copy of a multidimensional matrix. Matrix elements
        ///     themselves are copied only in a shallowed manner (i.e. not cloned).
        /// </summary>
        public static T[,] MemberwiseClone<T>(this T[,] a)
        {
            // TODO: Rename to Copy and implement shallow and deep copies
            return (T[,])a.Clone();
        }

        /// <summary>
        ///     Creates a memberwise copy of a vector matrix. Vector elements
        ///     themselves are copied only in a shallow manner (i.e. not cloned).
        /// </summary>
        public static T[] MemberwiseClone<T>(this T[] a)
        {
            // TODO: Rename to Copy and implement shallow and deep copies
            return (T[])a.Clone();
        }

        /// <summary>
        ///     Creates a memberwise copy of a matrix. Matrix elements
        ///     themselves are copied only in a shallow manner (i.e. not cloned).
        /// </summary>
        public static T[,] Copy<T>(this T[,] a)
        {
            return (T[,])a.Clone();
        }

        /// <summary>
        ///     Copies the content of an array to another array.
        /// </summary>
        /// <typeparam name="T">The type of the elements to be copied.</typeparam>
        /// <param name="matrix">The source matrix to be copied.</param>
        /// <param name="destination">The matrix where the elements should be copied to.</param>
        /// <param name="transpose">Whether to transpose the matrix when copying or not. Default is false.</param>
        public static void CopyTo<T>(this T[,] matrix, T[,] destination, bool transpose = false)
        {
            if (matrix == destination)
            {
                if (transpose)
                    matrix.Transpose(true);
            }
            else
            {
                if (transpose)
                {
                    var rows = Math.Min(matrix.Rows(), destination.Columns());
                    var cols = Math.Min(matrix.Columns(), destination.Rows());
                    for (var i = 0; i < rows; i++)
                        for (var j = 0; j < cols; j++)
                            destination[j, i] = matrix[i, j];
                }
                else
                {
                    if (matrix.Length == destination.Length)
                    {
                        Array.Copy(matrix, 0, destination, 0, matrix.Length);
                    }
                    else
                    {
                        var rows = Math.Min(matrix.Rows(), destination.Rows());
                        var cols = Math.Min(matrix.Columns(), destination.Columns());
                        for (var i = 0; i < rows; i++)
                            for (var j = 0; j < cols; j++)
                                destination[i, j] = matrix[i, j];
                    }
                }
            }
        }

        /// <summary>
        ///     Copies the content of an array to another array.
        /// </summary>
        /// <typeparam name="T">The type of the elements to be copied.</typeparam>
        /// <param name="matrix">The source matrix to be copied.</param>
        /// <param name="destination">The matrix where the elements should be copied to.</param>
        /// <param name="transpose">Whether to transpose the matrix when copying or not. Default is false.</param>
        public static void CopyTo<T>(this T[,] matrix, T[][] destination, bool transpose = false)
        {
            if (transpose)
            {
                var rows = Math.Min(matrix.Rows(), destination.Columns());
                var cols = Math.Min(matrix.Columns(), destination.Rows());
                for (var i = 0; i < rows; i++)
                    for (var j = 0; j < cols; j++)
                        destination[j][i] = matrix[i, j];
            }
            else
            {
                if (matrix.Length == destination.Length)
                {
                    Array.Copy(matrix, 0, destination, 0, matrix.Length);
                }
                else
                {
                    var rows = Math.Min(matrix.Rows(), destination.Rows());
                    var cols = Math.Min(matrix.Columns(), destination.Columns());
                    for (var i = 0; i < rows; i++)
                        for (var j = 0; j < cols; j++)
                            destination[i][j] = matrix[i, j];
                }
            }
        }
        /// <summary>
        ///     Copies the content of an array to another array.
        /// </summary>
        /// <typeparam name="T">The type of the elements to be copied.</typeparam>
        /// <param name="matrix">The source matrix to be copied.</param>
        /// <param name="destination">The matrix where the elements should be copied to.</param>
        public static void CopyTo<T>(this T[,] matrix, T[][] destination)
        {
            for (var i = 0; i < destination.Length; i++)
                for (var j = 0; j < destination[i].Length; j++)
                    destination[i][j] = matrix[i, j];
        }

        /// <summary>
        ///     Copies the content of an array to another array.
        /// </summary>
        /// <typeparam name="T">The type of the elements to be copied.</typeparam>
        /// <param name="matrix">The source matrix to be copied.</param>
        /// <param name="destination">The matrix where the elements should be copied to.</param>
        public static void SetTo<T>(this T[] destination, T[] matrix)
        {
            if (matrix != destination)
                Array.Copy(matrix, 0, destination, 0, matrix.Length);
        }

        /// <summary>
        ///     Copies the content of an array to another array.
        /// </summary>
        /// <typeparam name="T">The type of the elements to be copied.</typeparam>
        /// <param name="destination">The matrix where the elements should be set.</param>
        /// <param name="value">The value to which the matrix elements should be set to.</param>
        public static void SetTo<T>(this T[] destination, T value)
        {
            for (var i = 0; i < destination.Length; i++)
                destination[i] = value;
        }

        /// <summary>
        ///     Copies the content of an array to another array.
        /// </summary>
        /// <typeparam name="T">The type of the elements to be copied.</typeparam>
        /// <param name="matrix">The source matrix to be copied.</param>
        /// <param name="destination">The matrix where the elements should be copied to.</param>
        public static void SetTo<T>(this T[,] destination, T[,] matrix)
        {
            Array.Copy(matrix, 0, destination, 0, matrix.Length);
        }
        /// <summary>
        ///     Copies the content of an array to another array.
        /// </summary>
        /// <typeparam name="T">The type of the elements to be copied.</typeparam>
        /// <param name="matrix">The source matrix to be copied.</param>
        /// <param name="destination">The matrix where the elements should be copied to.</param>
        public static void SetTo<T>(this T[,] destination, T[][] matrix)
        {
            for (var i = 0; i < matrix.Length; i++)
                for (var j = 0; j < matrix[i].Length; j++)
                    destination[i, j] = matrix[i][j];
        }

        /// <summary>
        ///     Copies the content of an array to another array.
        /// </summary>
        /// <typeparam name="T">The type of the elements to be copied.</typeparam>
        /// <param name="matrix">The source matrix to be copied.</param>
        /// <param name="destination">The matrix where the elements should be copied to.</param>
        public static void SetTo<T>(this T[][] destination, T[,] matrix)
        {
            for (var i = 0; i < destination.Length; i++)
                for (var j = 0; j < destination[i].Length; j++)
                    destination[i][j] = matrix[i, j];
        }

        /// <summary>
        ///     Sets all elements of an array to a given value.
        /// </summary>
        /// <typeparam name="T">The type of the elements to be copied.</typeparam>
        /// <param name="value">The value to be copied.</param>
        /// <param name="destination">The matrix where the elements should be copied to.</param>
        public static void SetTo<T>(this T[,] destination, T value)
        {
            for (var i = 0; i < destination.GetLength(0); i++)
                for (var j = 0; j < destination.GetLength(1); j++)
                    destination[i, j] = value;
        }
        /// <summary>
        ///     Sets all elements of an array to a given value.
        /// </summary>
        /// <typeparam name="T">The type of the elements to be copied.</typeparam>
        /// <param name="value">The value to be copied.</param>
        /// <param name="destination">The matrix where the elements should be copied to.</param>
        public static void SetTo<T>(this T[][] destination, T value)
        {
            for (var i = 0; i < destination.Length; i++)
                for (var j = 0; j < destination[i].Length; j++)
                    destination[i][j] = value;
        }

        /// <summary>
        ///     Sets all elements in an array to zero.
        /// </summary>
        public static void Clear(this Array array)
        {
            Array.Clear(array, 0, array.Length);
        }

        /// <summary>
        ///     Sets all elements in an array to zero.
        /// </summary>
        public static void Clear<T>(this T[][] array)
        {
            for (var i = 0; i < array.Length; i++)
                Array.Clear(array[i], 0, array[i].Length);
        }

        /// <summary>
        ///     Replaces one value by another in a matrix of any dimensions.
        ///     This is not an optimized operation.
        /// </summary>
        /// <param name="array">The array where elements will be replaced.</param>
        /// <param name="from">The values which should be replaced.</param>
        /// <param name="to">The value to put in place of <paramref name="from" />.</param>
        /// <returns>
        ///     A new array where all instances of <paramref name="from" />
        ///     have been replaced with <paramref name="to" />.
        /// </returns>
        public static T Replace<T>(this T array, object from, object to)
            where T : class
        {
            var ar = array as Array;
            var result = (Array)ar.Clone();
            foreach (var dim in ar.GetIndices())
                if (result.GetValue(dim) == from)
                    result.SetValue(to, dim);
            return result as T;
        }
        #region Comparison

        /// <summary>
        ///     Determines whether a number is an integer, given a tolerance threshold.
        /// </summary>
        /// <param name="x">The value to be compared.</param>
        /// <param name="threshold">The maximum that the number can deviate from its closest integer number.</param>
        /// <returns>True if the number if an integer, false otherwise.</returns>
        public static bool IsInteger(this double x, double threshold = Constants.DoubleSmall)
        {
            var a = Math.Round(x);
            var b = x;

            if (a == b)
                return true;

            var limit = Math.Abs(a) * threshold;
            var delta = Math.Abs(a - b);

            return delta <= limit;
        }

        /// <summary>
        ///     Compares two objects for equality, performing an elementwise
        ///     comparison if the elements are vectors or matrices.
        /// </summary>
        public static bool IsEqual(this object objA, object objB, decimal atol = 0, decimal rtol = 0)
        {
            if (Equals(objA, objB))
                return true;

            if (objA is DBNull)
                objA = null;

            if (objB is DBNull)
                objB = null;

            if ((objA == null) ^ (objB == null))
                return false;

            try
            {
                var a = System.Convert.ToDecimal(objA);
                var b = System.Convert.ToDecimal(objB);
                if (a == b)
                    return true;
                if (atol > 0)
                    return Math.Abs(a - b) < atol;
                if (rtol > 0)
                    return Math.Abs(a - b) < rtol * b;
            }
            catch
            {
            } // TODO: Remove this try-catch block

            return false;
        }

        /// <summary>
        ///     Compares two matrices for equality.
        /// </summary>
        public static bool IsEqual(this Array objA, Array objB, double atol = 0, double rtol = 0)
        {
            if (objA == objB)
                return true;

            if (objA == null)
                throw new ArgumentNullException("objA");

            if (objB == null)
                throw new ArgumentNullException("objB");

            if (!objA.GetLength().IsEqual(objB.GetLength()))
                return false;

            // TODO: Implement this cache mechanism here
            // http://blog.slaks.net/2015-06-26/code-snippets-fast-property-access-reflection/

            // Check if there is already an optimized method to perform this comparison
            var typeA = objA.GetType();
            var typeB = objB.GetType();

            var equals = typeof(Matrix).GetMethod("IsEqual", new[]
            {
                typeA, typeB, typeof(double), typeof(double)
            });

            var _this = typeof(Matrix).GetMethod("IsEqual", new[]
            {
                typeof(Array), typeof(Array), typeof(double), typeof(double)
            });

            if (equals != _this)
                return (bool)equals.Invoke(null, new object[] { objA, objB, atol, rtol });

            // Base case: arrays contain elements of same nature (both arrays, or both values)
            if (typeA.GetElementType().IsArray == typeB.GetElementType().IsArray)
            {
                var a = objA.GetEnumerator();
                var b = objB.GetEnumerator();

                while (a.MoveNext() && b.MoveNext())
                {
                    if (a.Current == b.Current)
                        continue;

                    var arrA = a.Current as Array;
                    var arrB = b.Current as Array;
                    if (arrA != null && arrB != null && IsEqual(arrA, arrB, atol, rtol))
                        continue;

                    if (!IsEqual(a.Current, b.Current, (decimal)atol, (decimal)rtol))
                        return false;
                }

                return true;
            }

            // Arrays contain mixed elements (i.e. one is jagged and other multidimensional)
            foreach (int[] idx in Matrix.GetIndices(objA, deep: true, max: true))
            {
                object a = 0, b = 0;
                objA.TryGetValue(true, idx, out a);
                objB.TryGetValue(true, idx, out b);

                if (!IsEqual(a, b, (decimal)atol, (decimal)rtol))
                    return false;
            }

            return true;
        }

        /// <summary>
        ///     This method should not be called. Use Matrix.IsEqual instead.
        /// </summary>
        public new static bool Equals(object value)
        {
            throw new NotSupportedException("Use Matrix.IsEqual instead.");
        }

        /// <summary>
        ///     Checks whether two arrays have the same dimensions.
        /// </summary>
        public static bool DimensionEquals(this Array a, Array b)
        {
            if (a.Rank != b.Rank)
                return false;
            for (var i = 0; i < a.Rank; i++)
                if (a.GetLength(i) != b.GetLength(i))
                    return false;
            return true;
        }

        /// <summary>
        ///     Compares two enumerables for set equality. Two
        ///     enumerables are set equal if they contain the
        ///     same elements, but not necessarily in the same
        ///     order.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="list1">The first set.</param>
        /// <param name="list2">The first set.</param>
        /// <returns>
        ///     True if the two sets contains the same elements, false otherwise.
        /// </returns>
        public static bool SetEquals<T>(this IEnumerable<T> list1, IEnumerable<T> list2)
        {
            var cnt = new Dictionary<T, int>();

            foreach (var s in list1)
                if (cnt.ContainsKey(s))
                    cnt[s]++;
                else cnt.Add(s, 1);

            foreach (var s in list2)
                if (cnt.ContainsKey(s))
                    cnt[s]--;
                else return false;

            foreach (var s in cnt)
                if (s.Value != 0)
                    return false;

            return true;
        }

        /// <summary>
        ///     Returns a value indicating whether the specified
        ///     matrix contains a value that is not a number (NaN).
        /// </summary>
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// <returns>True if the matrix contains a value that is not a number, false otherwise.</returns>
        public static bool HasNaN(this double[,] matrix)
        {
            foreach (var e in matrix)
                if (double.IsNaN(e))
                    return true;
            return false;
        }

        /// <summary>
        ///     Returns a value indicating whether the specified
        ///     matrix contains a value that is not a number (NaN).
        /// </summary>
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// <returns>True if the matrix contains a value that is not a number, false otherwise.</returns>
        public static bool HasNaN(this double[] matrix)
        {
            foreach (var e in matrix)
                if (double.IsNaN(e))
                    return true;
            return false;
        }

        /// <summary>
        ///     Returns a value indicating whether the specified
        ///     matrix contains a value that is not a number (NaN).
        /// </summary>
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// <returns>True if the matrix contains a value that is not a number, false otherwise.</returns>
        public static bool HasNaN(this double[][] matrix)
        {
            for (var i = 0; i < matrix.Length; i++)
                for (var j = 0; j < matrix[i].Length; j++)
                    if (double.IsNaN(matrix[i][j]))
                        return true;
            return false;
        }

        /// <summary>
        ///     Returns a value indicating whether the specified
        ///     matrix contains a infinity value.
        /// </summary>
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// <returns>True if the matrix contains infinity values, false otherwise.</returns>
        public static bool HasInfinity(this double[,] matrix)
        {
            foreach (var e in matrix)
                if (double.IsInfinity(e))
                    return true;
            return false;
        }
        /// <summary>
        ///     Returns a value indicating whether the specified
        ///     matrix contains a value within a given tolerance.
        /// </summary>
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// <param name="value">The value to search for in the matrix.</param>
        /// <param name="tolerance">
        ///     The relative tolerance that a value must be in
        ///     order to be considered equal to the value being searched.
        /// </param>
        /// <returns>True if the matrix contains the value, false otherwise.</returns>
        public static bool Has(this double[,] matrix, double value, double tolerance = 0.0)
        {
            foreach (var e in matrix)
                if (Math.Abs(e - value) <= Math.Abs(e) * tolerance)
                    return true;
            return false;
        }

        /// <summary>
        ///     Returns a value indicating whether the specified
        ///     matrix contains a value within a given tolerance.
        /// </summary>
        /// <param name="matrix">A single-precision multidimensional matrix.</param>
        /// <param name="value">The value to search for in the matrix.</param>
        /// <param name="tolerance">
        ///     The relative tolerance that a value must be in
        ///     order to be considered equal to the value being searched.
        /// </param>
        /// <returns>True if the matrix contains the value, false otherwise.</returns>
        public static bool Has(this float[,] matrix, float value, double tolerance = 0.0)
        {
            foreach (var e in matrix)
                if (Math.Abs(e - value) <= Math.Abs(e) * tolerance)
                    return true;
            return false;
        }

        /// <summary>
        ///     Returns a value indicating whether the specified
        ///     matrix contains a infinity value.
        /// </summary>
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// <returns>True if the matrix contains a infinity value, false otherwise.</returns>
        public static bool HasInfinity(this double[] matrix)
        {
            foreach (var e in matrix)
                if (double.IsInfinity(e))
                    return true;
            return false;
        }

        /// <summary>
        ///     Returns a value indicating whether the specified
        ///     matrix contains a infinity value.
        /// </summary>
        /// <param name="matrix">A double-precision multidimensional matrix.</param>
        /// <returns>True if the matrix contains a infinity value, false otherwise.</returns>
        public static bool HasInfinity(this double[][] matrix)
        {
            for (var i = 0; i < matrix.Length; i++)
                for (var j = 0; j < matrix[i].Length; j++)
                    if (double.IsInfinity(matrix[i][j]))
                        return true;
            return false;
        }

        #endregion
        #region Transpose

        /// <summary>
        ///     Gets the transpose of a matrix.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <returns>The transpose of the given matrix.</returns>
        public static T[,] Transpose<T>(this T[,] matrix)
        {
            return Transpose(matrix, false);
        }

        /// <summary>
        ///     Gets the transpose of a matrix.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <param name="inPlace">
        ///     True to store the transpose over the same input
        ///     <paramref name="matrix" />, false otherwise. Default is false.
        /// </param>
        /// <returns>The transpose of the given matrix.</returns>
        public static T[,] Transpose<T>(this T[,] matrix, bool inPlace)
        {
            var rows = matrix.GetLength(0);
            var cols = matrix.GetLength(1);

            if (inPlace)
            {
                if (rows != cols)
                    throw new ArgumentException("Only square matrices can be transposed in place.", "matrix");

#if DEBUG
                var expected = matrix.Transpose();
#endif

                for (var i = 0; i < rows; i++)
                    for (var j = i; j < cols; j++)
                    {
                        var element = matrix[j, i];
                        matrix[j, i] = matrix[i, j];
                        matrix[i, j] = element;
                    }

#if DEBUG
                if (!expected.IsEqual(matrix))
                    throw new Exception();
#endif

                return matrix;
            }

            var result = new T[cols, rows];
            for (var i = 0; i < rows; i++)
                for (var j = 0; j < cols; j++)
                    result[j, i] = matrix[i, j];

            return result;
        }
        /// <summary>
        ///     Gets the transpose of a row vector.
        /// </summary>
        /// <param name="rowVector">A row vector.</param>
        /// <returns>The transpose of the given vector.</returns>
        public static T[,] Transpose<T>(this T[] rowVector)
        {
            var result = new T[rowVector.Length, 1];
            for (var i = 0; i < rowVector.Length; i++)
                result[i, 0] = rowVector[i];
            return result;
        }

        /// <summary>
        ///     Gets the transpose of a row vector.
        /// </summary>
        /// <param name="rowVector">A row vector.</param>
        /// <param name="result">The matrix where to store the transpose.</param>
        /// <returns>The transpose of the given vector.</returns>
        public static T[,] Transpose<T>(this T[] rowVector, T[,] result)
        {
            for (var i = 0; i < rowVector.Length; i++)
                result[i, 0] = rowVector[i];
            return result;
        }
        /// <summary>
        ///     Gets the generalized transpose of a tensor.
        /// </summary>
        /// <param name="array">A tensor.</param>
        /// <returns>The transpose of the given tensor.</returns>
        public static Array Transpose(this Array array)
        {
            return Transpose(array, Vector.Range(array.Rank - 1, -1));
        }

        /// <summary>
        ///     Gets the generalized transpose of a tensor.
        /// </summary>
        /// <param name="array">A tensor.</param>
        /// <param name="order">The new order for the tensor's dimensions.</param>
        /// <returns>The transpose of the given tensor.</returns>
        public static Array Transpose(this Array array, int[] order)
        {
            return transpose(array, order);
        }

        /// <summary>
        ///     Gets the generalized transpose of a tensor.
        /// </summary>
        /// <param name="array">A tensor.</param>
        /// <param name="order">The new order for the tensor's dimensions.</param>
        /// <returns>The transpose of the given tensor.</returns>
        public static T Transpose<T>(this T array, int[] order)
            where T : class, IList
        {
            var arr = array as Array;

            if (arr == null)
                throw new ArgumentException("The given object must inherit from System.Array.", "array");

            return transpose(arr, order) as T;
        }

        private static Array transpose(Array array, int[] order)
        {
            if (order.Length != array.Rank)
                throw new ArgumentException("order");

            if (array.Length == 1 || array.Length == 0)
                return array;

            // Get the number of samples at each dimension
            var size = new int[array.Rank];
            for (var i = 0; i < size.Length; i++)
                size[i] = array.GetLength(i);

            var r = Array.CreateInstance(array.GetType().GetElementType(), size.Get(order));

            // Generate all indices for accessing the matrix 
            foreach (var pos in size.Sequences(true))
            {
                var newPos = pos.Get(order);
                var value = array.GetValue(pos);
                r.SetValue(value, newPos);
            }

            return r;
        }

        #endregion
        #region Matrix Characteristics

        /// <summary>
        ///     Gets the total number of elements in the vector.
        /// </summary>
        public static int GetNumberOfElements<T>(this T[] value)
        {
            return value.Length;
        }

        /// <summary>
        ///     Gets the total number of elements in the matrix.
        /// </summary>
        public static int GetNumberOfElements<T>(this T[][] value)
        {
            var sum = 0;
            for (var i = 0; i < value.Length; i++)
                sum += value[i].Length;
            return sum;
        }

        /// <summary>
        ///     Gets the total number of elements in the matrix.
        /// </summary>
        public static int GetNumberOfElements<T>(this T[,] elements)
        {
            return elements.GetLength().Sum();
        }

        /// <summary>
        ///     Gets the size of a vector, in bytes.
        /// </summary>
        public static int GetSizeInBytes<T>(this T[] elements)
        {
            return elements.GetNumberOfElements() * Marshal.SizeOf(typeof(T));
        }

        /// <summary>
        ///     Gets the size of a matrix, in bytes.
        /// </summary>
        public static int GetSizeInBytes<T>(this T[][] elements)
        {
            return elements.GetNumberOfElements() * Marshal.SizeOf(typeof(T));
        }

        /// <summary>
        ///     Gets the size of a matrix, in bytes.
        /// </summary>
        public static int GetSizeInBytes<T>(this T[,] elements)
        {
#if NETSTANDARD1_4
            return elements.GetNumberOfElements() * Marshal.SizeOf<T>();
#else
            return elements.GetNumberOfElements() * Marshal.SizeOf(typeof(T));
#endif
        }

        /// <summary>
        ///     Gets the number of rows in a vector.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the column vector.</typeparam>
        /// <param name="vector">The vector whose number of rows must be computed.</param>
        /// <returns>The number of rows in the column vector.</returns>
        public static int Rows<T>(this T[] vector)
        {
            return vector.Length;
        }

        /// <summary>
        ///     Gets the number of rows in a multidimensional matrix.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
        /// <param name="matrix">The matrix whose number of rows must be computed.</param>
        /// <returns>The number of rows in the matrix.</returns>
        public static int Rows<T>(this T[,] matrix)
        {
            return matrix.GetLength(0);
        }

        /// <summary>
        ///     Gets the number of columns in a multidimensional matrix.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
        /// <param name="matrix">The matrix whose number of columns must be computed.</param>
        /// <returns>The number of columns in the matrix.</returns>
        public static int Columns<T>(this T[,] matrix)
        {
            return matrix.GetLength(1);
        }

        /// <summary>
        ///     Gets the number of rows in a multidimensional matrix.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
        /// <param name="matrix">The matrix whose number of rows must be computed.</param>
        /// <returns>The number of rows in the matrix.</returns>
        public static int Rows<T>(this T[,,] matrix)
        {
            return matrix.GetLength(0);
        }

        /// <summary>
        ///     Gets the number of columns in a multidimensional matrix.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
        /// <param name="matrix">The matrix whose number of columns must be computed.</param>
        /// <returns>The number of columns in the matrix.</returns>
        public static int Columns<T>(this T[,,] matrix)
        {
            return matrix.GetLength(1);
        }

        /// <summary>
        ///     Gets the number of columns in a multidimensional matrix.
        /// </summary>
        /// <typeparam name="T">The type of the elements in the matrix.</typeparam>
        /// <param name="matrix">The matrix whose number of columns must be computed.</param>
        /// <returns>The number of columns in the matrix.</returns>
        public static int Depth<T>(this T[,,] matrix)
        {
            return matrix.GetLength(2);
        }

        /// <summary>
        ///     Returns true if a vector of real-valued observations
        ///     is ordered in ascending or descending order.
        /// </summary>
        /// <param name="values">An array of values.</param>
        public static bool IsSorted<T>(this T[] values)
            where T : IComparable<T>
        {
            return IsSorted(values, ComparerDirection.Ascending)
                   || IsSorted(values, ComparerDirection.Descending);
        }

        /// <summary>
        ///     Returns true if a vector of real-valued observations
        ///     is ordered in ascending or descending order.
        /// </summary>
        /// <param name="values">An array of values.</param>
        /// <param name="direction">The sort order direction.</param>
        public static bool IsSorted<T>(this T[] values, ComparerDirection direction)
            where T : IComparable<T>
        {
            if (direction == ComparerDirection.Ascending)
            {
                for (var i = 1; i < values.Length; i++)
                    if (values[i - 1].CompareTo(values[i]) > 0)
                        return false;
            }
            else
            {
                for (var i = 1; i < values.Length; i++)
                    if (values[i - 1].CompareTo(values[i]) < 0)
                        return false;
            }

            return true;
        }

        /// <summary>
        ///     Returns true if a matrix is square.
        /// </summary>
        public static bool IsSquare<T>(this T[][] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            return matrix.Rows() == matrix.Columns(true);
        }

        /// <summary>
        ///     Returns true if a matrix is square.
        /// </summary>
        public static bool IsSquare<T>(this T[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            return matrix.Rows() == matrix.Columns();
        }
        /// <summary>
        ///     Returns true if a matrix is upper triangular.
        /// </summary>
        public static bool IsUpperTriangular<T>(this T[,] matrix) where T : IComparable
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            var zero = default(T);

            if (matrix.GetLength(0) != matrix.GetLength(1))
                return false;

            for (var i = 0; i < matrix.GetLength(0); i++)
                for (var j = 0; j < i; j++)
                    if (matrix[i, j].CompareTo(zero) != 0)
                        return false;

            return true;
        }

        /// <summary>
        ///     Returns true if a matrix is lower triangular.
        /// </summary>
        public static bool IsLowerTriangular<T>(this T[,] matrix) where T : IComparable
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            var zero = default(T);

            if (matrix.GetLength(0) != matrix.GetLength(1))
                return false;

            for (var i = 0; i < matrix.GetLength(0); i++)
                for (var j = i + 1; j < matrix.GetLength(1); j++)
                    if (matrix[i, j].CompareTo(zero) != 0)
                        return false;

            return true;
        }

        /// <summary>
        ///     Converts a matrix to lower triangular form, if possible.
        /// </summary>
        public static T[,] ToLowerTriangular<T>(this T[,] matrix, MatrixType from, T[,] result = null)
        {
            if (result == null)
                result = Matrix.CreateAs(matrix);
            matrix.CopyTo(result);

            switch (from)
            {
                case MatrixType.LowerTriangular:
                case MatrixType.Diagonal:
                    break;

                case MatrixType.UpperTriangular:
                    Transpose(result, true);
                    break;

                default:
                    throw new ArgumentException(
                        "Only LowerTriangular, UpperTriangular and Diagonal matrices are supported at this time.",
                        "matrixType");
            }

            return result;
        }

        /// <summary>
        ///     Converts a matrix to upper triangular form, if possible.
        /// </summary>
        public static T[,] ToUpperTriangular<T>(this T[,] matrix, MatrixType from, T[,] result = null)
        {
            if (result == null)
                result = Matrix.CreateAs(matrix);
            matrix.CopyTo(result);

            switch (from)
            {
                case MatrixType.UpperTriangular:
                case MatrixType.Diagonal:
                    break;

                case MatrixType.LowerTriangular:
                    Transpose(result, true);
                    break;

                default:
                    throw new ArgumentException(
                        "Only LowerTriangular, UpperTriangular and Diagonal matrices are supported at this time.",
                        "matrixType");
            }

            return result;
        }

        /// <summary>
        ///     Converts a matrix to lower triangular form, if possible.
        /// </summary>
        public static T[][] ToLowerTriangular<T>(this T[][] matrix, MatrixType from, T[][] result = null)
        {
            if (result == null)
                result = Jagged.CreateAs(matrix);
            matrix.CopyTo(result);

            switch (from)
            {
                case MatrixType.LowerTriangular:
                case MatrixType.Diagonal:
                    break;

                case MatrixType.UpperTriangular:
                    Transpose(result, true);
                    break;

                default:
                    throw new ArgumentException(
                        "Only LowerTriangular, UpperTriangular and Diagonal matrices are supported at this time.",
                        "matrixType");
            }

            return result;
        }

        /// <summary>
        ///     Converts a matrix to upper triangular form, if possible.
        /// </summary>
        public static T[][] ToUpperTriangular<T>(this T[][] matrix, MatrixType from, T[][] result = null)
        {
            if (result == null)
                result = Jagged.CreateAs(matrix);
            matrix.CopyTo(result);

            switch (from)
            {
                case MatrixType.UpperTriangular:
                case MatrixType.Diagonal:
                    break;

                case MatrixType.LowerTriangular:
                    Transpose(result, true);
                    break;

                default:
                    throw new ArgumentException(
                        "Only LowerTriangular, UpperTriangular and Diagonal matrices are supported at this time.",
                        "matrixType");
            }

            return result;
        }

        /// <summary>
        ///     Gets the lower triangular part of a matrix.
        /// </summary>
        public static T[,] GetLowerTriangle<T>(this T[,] matrix, bool includeDiagonal = true)
        {
            var s = includeDiagonal ? 1 : 0;
            var r = Matrix.CreateAs(matrix);
            for (var i = 0; i < matrix.Rows(); i++)
                for (var j = 0; j < i + s; j++)
                    r[i, j] = matrix[i, j];
            ;
            return r;
        }

        /// <summary>
        ///     Gets the upper triangular part of a matrix.
        /// </summary>
        public static T[,] GetUpperTriangle<T>(this T[,] matrix, bool includeDiagonal = false)
        {
            var s = includeDiagonal ? 0 : 1;
            var r = Matrix.CreateAs(matrix);
            for (var i = 0; i < matrix.Rows(); i++)
                for (var j = i + s; j < matrix.Columns(); j++)
                    r[i, j] = matrix[i, j];
            ;
            return r;
        }

        /// <summary>
        ///     Transforms a triangular matrix in a symmetric matrix by copying
        ///     its elements to the other, unfilled part of the matrix.
        /// </summary>
        public static T[,] GetSymmetric<T>(this T[,] matrix, MatrixType type, T[,] result = null)
        {
            if (result == null)
                result = Matrix.CreateAs(matrix);

            switch (type)
            {
                case MatrixType.LowerTriangular:
                    for (var i = 0; i < matrix.Rows(); i++)
                        for (var j = 0; j <= i; j++)
                            result[i, j] = result[j, i] = matrix[i, j];
                    break;
                case MatrixType.UpperTriangular:
                    for (var i = 0; i < matrix.Rows(); i++)
                        for (var j = i; j < matrix.Columns(); j++)
                            result[i, j] = result[j, i] = matrix[i, j];
                    break;
                default:
                    throw new Exception("Matrix type can be either LowerTriangular or UpperTrianguler.");
            }

            return result;
        }

        /// <summary>
        ///     Returns true if a matrix is diagonal
        /// </summary>
        public static bool IsDiagonal<T>(this T[,] matrix) where T : IComparable
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            var zero = default(T);

            if (matrix.GetLength(0) != matrix.GetLength(1))
                return false;

            for (var i = 0; i < matrix.GetLength(0); i++)
                for (var j = 0; j < matrix.GetLength(1); j++)
                    if (i != j && matrix[i, j].CompareTo(zero) != 0)
                        return false;

            return true;
        }

        /// <summary>
        ///     Gets the trace of a matrix.
        /// </summary>
        /// <remarks>
        ///     The trace of an n-by-n square matrix A is defined to be the sum of the
        ///     elements on the main diagonal (the diagonal from the upper left to the
        ///     lower right) of A.
        /// </remarks>
        public static double Trace(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            var rows = matrix.GetLength(0);

            var trace = 0.0;
            for (var i = 0; i < rows; i++)
                trace += matrix[i, i];
            return trace;
        }

        /// <summary>
        ///     Gets the trace of a matrix product.
        /// </summary>
        public static double Trace(double[,] matrixA, double[,] matrixB)
        {
            if (matrixA.Length != matrixB.Length)
                throw new DimensionMismatchException("matrixB", "Matrices must have the same length.");

            var length = matrixA.Length;

            unsafe
            {
                fixed (double* ptrA = matrixA)
                fixed (double* ptrB = matrixB)
                {
                    var a = ptrA;
                    var b = ptrB;

                    var trace = 0.0;
                    for (var i = 0; i < length; i++)
                        trace += *a++ * *b++;
                    return trace;
                }
            }
        }

        /// <summary>
        ///     Gets the trace of a matrix.
        /// </summary>
        /// <remarks>
        ///     The trace of an n-by-n square matrix A is defined to be the sum of the
        ///     elements on the main diagonal (the diagonal from the upper left to the
        ///     lower right) of A.
        /// </remarks>
        public static int Trace(this int[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            var rows = matrix.GetLength(0);

            var trace = 0;
            for (var i = 0; i < rows; i++)
                trace += matrix[i, i];
            return trace;
        }

        /// <summary>
        ///     Gets the trace of a matrix.
        /// </summary>
        /// <remarks>
        ///     The trace of an n-by-n square matrix A is defined to be the sum of the
        ///     elements on the main diagonal (the diagonal from the upper left to the
        ///     lower right) of A.
        /// </remarks>
        public static float Trace(this float[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            var rows = matrix.GetLength(0);

            var trace = 0.0f;
            for (var i = 0; i < rows; i++)
                trace += matrix[i, i];
            return trace;
        }

        /// <summary>
        ///     Gets the diagonal vector from a matrix.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <returns>The diagonal vector from the given matrix.</returns>
        public static T[] Diagonal<T>(this T[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            var r = new T[matrix.GetLength(0)];
            for (var i = 0; i < r.Length; i++)
                r[i] = matrix[i, i];

            return r;
        }

        /// <summary>
        ///     Gets the determinant of a matrix.
        /// </summary>
        public static double Determinant(this double[,] matrix)
        {
            // Assume the most general case
            return Determinant(matrix, false);
        }

        /// <summary>
        ///     Gets the determinant of a matrix.
        /// </summary>
        public static double Determinant(this double[,] matrix, bool symmetric)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            if (symmetric) // Use faster robust Cholesky decomposition
            {
                var chol = new CholeskyDecomposition(matrix, true);

                if (!chol.IsPositiveDefinite)
                    throw new ArgumentException("The matrix could not be decomposed using " +
                                                " a robust Cholesky decomposition. Please specify symmetric as false " +
                                                " and provide a full matrix to be decomposed.", "matrix");

                return chol.Determinant;
            }

            return new LuDecomposition(matrix).Determinant;
        }

        /// <summary>
        ///     Gets the log-determinant of a matrix.
        /// </summary>
        public static double LogDeterminant(this double[,] matrix)
        {
            // Assume the most general case
            return LogDeterminant(matrix, false);
        }

        /// <summary>
        ///     Gets the log-determinant of a matrix.
        /// </summary>
        public static double LogDeterminant(this double[,] matrix, bool symmetric)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            if (symmetric) // Use faster robust Cholesky decomposition
            {
                var chol = new CholeskyDecomposition(matrix, true);

                if (!chol.IsPositiveDefinite)
                    throw new ArgumentException("The matrix could not be decomposed using " +
                                                " a robust Cholesky decomposition. Please specify symmetric as false " +
                                                " and provide a full matrix to be decomposed.", "matrix");

                return chol.LogDeterminant;
            }

            return new LuDecomposition(matrix).LogDeterminant;
        }

        /// <summary>
        ///     Gets the pseudo-determinant of a matrix.
        /// </summary>
        public static double PseudoDeterminant(this double[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            return new SingularValueDecomposition(matrix,
                false, false,
                true, false).PseudoDeterminant;
        }

        /// <summary>
        ///     Gets the pseudo-determinant of a matrix.
        /// </summary>
        public static double PseudoDeterminant(this double[][] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            return new JaggedSingularValueDecomposition(matrix,
                false, false,
                true, false).PseudoDeterminant;
        }

        /// <summary>
        ///     Gets the log of the pseudo-determinant of a matrix.
        /// </summary>
        public static double LogPseudoDeterminant(this double[,] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            return new SingularValueDecomposition(matrix,
                false, false,
                true, false).LogPseudoDeterminant;
        }

        /// <summary>
        ///     Gets the log of the pseudo-determinant of a matrix.
        /// </summary>
        public static double LogPseudoDeterminant(this double[][] matrix)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            return new JaggedSingularValueDecomposition(matrix,
                false, false,
                true, false).LogPseudoDeterminant;
        }
        /// <summary>
        ///     Gets the rank of a matrix.
        /// </summary>
        public static int Rank(this double[,] matrix)
        {
            return new SingularValueDecomposition(matrix,
                false, false,
                true, false).Rank;
        }

        /// <summary>
        ///     Gets whether a matrix is singular.
        /// </summary>
        public static bool IsSingular(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");
            return new SingularValueDecomposition(matrix).IsSingular;
        }

        /// <summary>
        ///     Gets whether a matrix is positive definite.
        /// </summary>
        public static bool IsPositiveDefinite(this double[,] matrix)
        {
            if (matrix == null) throw new ArgumentNullException("matrix");

            return new CholeskyDecomposition(matrix).IsPositiveDefinite;
        }

        #endregion

        #region Operation Mapping (Apply)

        /// <summary>
        ///     Applies a function to every element of the array.
        /// </summary>
        public static TResult[] Apply<TInput, TResult>(this TInput[] vector, Func<TInput, TResult> func)
        {
            return Apply(vector, func, new TResult[vector.Length]);
        }

        /// <summary>
        ///     Applies a function to every element of the array.
        /// </summary>
        public static TResult[] Apply<TInput, TResult>(this TInput[] vector, Func<TInput, int, TResult> func)
        {
            return Apply(vector, func, new TResult[vector.Length]);
        }

        /// <summary>
        ///     Applies a function to every element of the array.
        /// </summary>
        public static TResult[] Apply<TInput, TResult>(this TInput[] vector, Func<TInput, TResult> func,
            TResult[] result)
        {
            for (var i = 0; i < vector.Length; i++)
                result[i] = func(vector[i]);
            return result;
        }

        /// <summary>
        ///     Applies a function to every element of the array.
        /// </summary>
        public static TResult[] Apply<TInput, TResult>(this TInput[] vector, Func<TInput, int, TResult> func,
            TResult[] result)
        {
            for (var i = 0; i < vector.Length; i++)
                result[i] = func(vector[i], i);
            return result;
        }
        /// <summary>
        ///     Applies a function to every element of the array.
        /// </summary>
        public static TResult[,] Apply<TInput, TResult>(this TInput[,] matrix, Func<TInput, TResult> func)
        {
            return Apply(matrix, func, Matrix.CreateAs<TInput, TResult>(matrix));
        }

        /// <summary>
        ///     Applies a function to every element of the array.
        /// </summary>
        public static TResult[,] Apply<TInput, TResult>(this TInput[,] matrix, Func<TInput, int, int, TResult> func)
        {
            return Apply(matrix, func, Matrix.CreateAs<TInput, TResult>(matrix));
        }

        /// <summary>
        ///     Applies a function to every element of the array.
        /// </summary>
        public static TResult[,] Apply<TInput, TResult>(this TInput[,] matrix, Func<TInput, TResult> func,
            TResult[,] result)
        {
            for (var i = 0; i < matrix.GetLength(0); i++)
                for (var j = 0; j < matrix.GetLength(1); j++)
                    result[i, j] = func(matrix[i, j]);
            return result;
        }

        /// <summary>
        ///     Applies a function to every element of the array.
        /// </summary>
        public static TResult[,] Apply<TInput, TResult>(this TInput[,] matrix, Func<TInput, int, int, TResult> func,
            TResult[,] result)
        {
            for (var i = 0; i < matrix.GetLength(0); i++)
                for (var j = 0; j < matrix.GetLength(1); j++)
                    result[i, j] = func(matrix[i, j], i, j);
            return result;
        }

        /// <summary>
        ///     Applies a function to every element of the array.
        /// </summary>
        public static TResult[] Apply<TData, TResult>(this IList<TData> vector, Func<TData, TResult> func)
        {
            return Apply(vector, func, new TResult[vector.Count]);
        }

        /// <summary>
        ///     Applies a function to every element of the array.
        /// </summary>
        public static TResult[] Apply<TData, TResult>(this IList<TData> vector, Func<TData, TResult> func,
            TResult[] result)
        {
            for (var i = 0; i < vector.Count; i++)
                result[i] = func(vector[i]);
            return result;
        }

        #endregion

        #region Rounding and discretization

        /// <summary>
        ///     Rounds a double-precision floating-point matrix to a specified number of fractional digits.
        /// </summary>
        public static double[,] Round(this double[,] matrix, int decimals = 0)
        {
            if (matrix == null)
                throw new ArgumentNullException("matrix");

            var rows = matrix.GetLength(0);
            var cols = matrix.GetLength(1);

            var result = new double[rows, cols];

            for (var i = 0; i < rows; i++)
                for (var j = 0; j < cols; j++)
                    result[i, j] = Math.Round(matrix[i, j], decimals);

            return result;
        }

        /// <summary>
        ///     Rounds a double-precision floating-point number array to a specified number of fractional digits.
        /// </summary>
        public static double[] Round(double[] vector, int decimals = 0)
        {
            if (vector == null)
                throw new ArgumentNullException("vector");

            var result = new double[vector.Length];
            for (var i = 0; i < result.Length; i++)
                result[i] = Math.Round(vector[i], decimals);
            return result;
        }
        #endregion

        #region Morphological operations

        /// <summary>
        ///     Transforms a jagged array matrix into a single vector.
        /// </summary>
        /// <param name="array">A jagged array.</param>
        public static Array DeepFlatten(this Array array)
        {
            var totalLength = array.GetTotalLength();
            var elementType = array.GetInnerMostType();
            var result = Array.CreateInstance(elementType, totalLength);

            var k = 0;
            foreach (var v in array.Enumerate())
                result.SetValue(v, k++);
            return result;
        }

        /// <summary>
        ///     Transforms a matrix into a single vector.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <param name="order">
        ///     The direction to perform copying. Pass
        ///     1 to perform a copy by reading the matrix in row-major order.
        ///     Pass 0 to perform a copy in column-major copy. Default is 1
        ///     (row-major, c-style order).
        /// </param>
        public static T[] Flatten<T>(this T[,] matrix, MatrixOrder order = MatrixOrder.Default)
        {
            return Reshape(matrix, order);
        }

        /// <summary>
        ///     Transforms a matrix into a single vector.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <param name="result">The vector where to store the copy.</param>
        /// <param name="order">
        ///     The direction to perform copying. Pass
        ///     1 to perform a copy by reading the matrix in row-major order.
        ///     Pass 0 to perform a copy in column-major copy. Default is 1
        ///     (row-major, c-style order).
        /// </param>
        public static T[] Flatten<T>(this T[,] matrix, T[] result, MatrixOrder order = MatrixOrder.Default)
        {
            return Reshape(matrix, result, order);
        }
        /// <summary>
        ///     Transforms a jagged array matrix into a single vector.
        /// </summary>
        /// <param name="array">A jagged array.</param>
        /// <param name="order">
        ///     The direction to perform copying. Pass
        ///     1 to perform a copy by reading the matrix in row-major order.
        ///     Pass 0 to perform a copy in column-major copy. Default is 1
        ///     (row-major, c-style order).
        /// </param>
        public static T[] Flatten<T>(this T[][] array, MatrixOrder order = MatrixOrder.Default)
        {
            return Reshape(array, order);
        }

        /// <summary>
        ///     Transforms a jagged array matrix into a single vector.
        /// </summary>
        /// <param name="array">A jagged array.</param>
        /// <param name="result">The vector where to store the copy.</param>
        /// <param name="order">
        ///     The direction to perform copying. Pass
        ///     1 to perform a copy by reading the matrix in row-major order.
        ///     Pass 0 to perform a copy in column-major copy. Default is 1
        ///     (row-major, c-style order).
        /// </param>
        public static T[] Flatten<T>(this T[][] array, T[] result, MatrixOrder order = MatrixOrder.Default)
        {
            return Reshape(array, result, order);
        }

        /// <summary>
        ///     Transforms a matrix into a single vector.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <param name="order">
        ///     The direction to perform copying. Pass
        ///     1 to perform a copy by reading the matrix in row-major order.
        ///     Pass 0 to perform a copy in column-major copy. Default is 1
        ///     (row-major, c-style order).
        /// </param>
        public static T[] Reshape<T>(this T[,] matrix, MatrixOrder order = MatrixOrder.Default)
        {
            var rows = matrix.GetLength(0);
            var cols = matrix.GetLength(1);
            return Reshape(matrix, new T[rows * cols], order);
        }

        /// <summary>
        ///     Transforms a matrix into a single vector.
        /// </summary>
        /// <param name="matrix">A matrix.</param>
        /// <param name="result">The vector where to store the copy.</param>
        /// <param name="order">
        ///     The direction to perform copying. Pass
        ///     1 to perform a copy by reading the matrix in row-major order.
        ///     Pass 0 to perform a copy in column-major copy. Default is 1
        ///     (row-major, c-style order).
        /// </param>
        public static T[] Reshape<T>(this T[,] matrix, T[] result, MatrixOrder order = MatrixOrder.Default)
        {
            var rows = matrix.GetLength(0);
            var cols = matrix.GetLength(1);

            if (order == MatrixOrder.CRowMajor)
            {
                var k = 0;
                for (var j = 0; j < rows; j++)
                    for (var i = 0; i < cols; i++)
                        result[k++] = matrix[j, i];
            }
            else
            {
                var k = 0;
                for (var i = 0; i < cols; i++)
                    for (var j = 0; j < rows; j++)
                        result[k++] = matrix[j, i];
            }

            return result;
        }
        /// <summary>
        ///     Transforms a jagged array matrix into a single vector.
        /// </summary>
        /// <param name="array">A jagged array.</param>
        /// <param name="order">
        ///     The direction to perform copying. Pass
        ///     1 to perform a copy by reading the matrix in row-major order.
        ///     Pass 0 to perform a copy in column-major copy. Default is 1
        ///     (row-major, c-style order).
        /// </param>
        public static T[] Reshape<T>(this T[][] array, MatrixOrder order = MatrixOrder.Default)
        {
            var count = 0;
            for (var i = 0; i < array.Length; i++)
                count += array[i].Length;
            return Reshape(array, new T[count], order);
        }

        /// <summary>
        ///     Transforms a jagged array matrix into a single vector.
        /// </summary>
        /// <param name="array">A jagged array.</param>
        /// <param name="result">The vector where to store the copy.</param>
        /// <param name="order">
        ///     The direction to perform copying. Pass
        ///     1 to perform a copy by reading the matrix in row-major order.
        ///     Pass 0 to perform a copy in column-major copy. Default is 1
        ///     (row-major, c-style order).
        /// </param>
        public static T[] Reshape<T>(this T[][] array, T[] result, MatrixOrder order = MatrixOrder.Default)
        {
            if (order == MatrixOrder.CRowMajor)
            {
                var k = 0;
                for (var j = 0; j < array.Length; j++)
                    for (var i = 0; i < array[j].Length; i++)
                        result[k++] = array[j][i];
            }
            else
            {
                var maxCols = 0;
                for (var i = 0; i < array.Length; i++)
                    if (array[i].Length > maxCols)
                        maxCols = array[i].Length;

                for (int i = 0, k = 0; i < maxCols; i++)
                    for (var j = 0; j < array.Length; j++)
                        if (i < array[j].Length)
                            result[k++] = array[j][i];
            }

            return result;
        }

        #endregion
    }
}