﻿using ISynergy.Framework.Core.Ranges;
using System;

namespace ISynergy.Framework.Mathematics
{
    public static partial class Matrix
    {
        internal static int GetLength<T>(T[][] values, int dimension)
        {
            if (dimension == 1)
                return values.Length;
            return values[0].Length;
        }

        internal static int GetLength<T>(T[,] values, int dimension)
        {
            if (dimension == 1)
                return values.GetLength(0);
            return values.GetLength(1);
        }
        /// <summary>
        ///     Gets the maximum and minimum values in a matrix.
        /// </summary>
        /// <param name="values">The vector whose min and max should be computed.</param>
        /// <param name="min">The minimum value in the vector.</param>
        /// <param name="max">The maximum value in the vector.</param>
        /// <exception cref="System.ArgumentException">Raised if the array is empty.</exception>
        public static void GetRange<T>(this T[][] values, out T min, out T max)
            where T : IComparable<T>
        {
            if (values.Length == 0 || values[0].Length == 0)
            {
                min = max = default;
                return;
            }

            min = max = values[0][0];
            for (var i = 0; i < values.Length; i++)
                for (var j = 0; j < values[i].Length; j++)
                {
                    if (values[i][j].CompareTo(min) < 0)
                        min = values[i][j];
                    if (values[i][j].CompareTo(max) > 0)
                        max = values[i][j];
                }
        }
        /// <summary>
        ///     Gets the range of the values across the columns of a matrix.
        /// </summary>
        /// <param name="value">The matrix whose ranges should be computed.</param>
        /// <param name="dimension">
        ///     Pass 0 if the range should be computed for each of the columns. Pass 1
        ///     if the range should be computed for each row. Default is 0.
        /// </param>
        public static NumericRange[] GetRange(this double[][] value, int dimension)
        {
            var rows = value.Length;
            var cols = value[0].Length;
            NumericRange[] ranges;

            if (dimension == 0)
            {
                ranges = new NumericRange[cols];

                for (var j = 0; j < ranges.Length; j++)
                {
                    var max = value[0][j];
                    var min = value[0][j];

                    for (var i = 0; i < rows; i++)
                    {
                        if (value[i][j] > max)
                            max = value[i][j];
                        if (value[i][j] < min)
                            min = value[i][j];
                    }

                    ranges[j] = new NumericRange(min, max);
                }
            }
            else
            {
                ranges = new NumericRange[rows];

                for (var j = 0; j < ranges.Length; j++)
                {
                    var max = value[j][0];
                    var min = value[j][0];

                    for (var i = 0; i < cols; i++)
                    {
                        if (value[j][i] > max)
                            max = value[j][i];
                        if (value[j][i] < min)
                            min = value[j][i];
                    }

                    ranges[j] = new NumericRange(min, max);
                }
            }

            return ranges;
        }
        #region Matrix ArgMin/ArgMax

        /// <summary>
        ///     Gets the index of the maximum element in a matrix.
        /// </summary>
        public static Tuple<int, int> ArgMax<T>(this T[][] matrix)
            where T : IComparable<T>
        {
            Tuple<int, int> index;
            Max(matrix, out index);
            return index;
        }

        /// <summary>
        ///     Gets the index of the maximum element in a matrix across a given dimension.
        /// </summary>
        public static int[] ArgMax<T>(this T[][] matrix, int dimension)
            where T : IComparable<T>
        {
            var s = GetLength(matrix, dimension);
            var values = new T[s];
            var indices = new int[s];
            Max(matrix, dimension, indices, values);
            return indices;
        }

        /// <summary>
        ///     Gets the index of the maximum element in a matrix across a given dimension.
        /// </summary>
        public static int[] ArgMax<T>(this T[][] matrix, int dimension, int[] result)
            where T : IComparable<T>
        {
            var s = GetLength(matrix, dimension);
            var values = new T[s];
            Max(matrix, dimension, result, values);
            return result;
        }
        /// <summary>
        ///     Gets the index of the minimum element in a matrix.
        /// </summary>
        public static Tuple<int, int> ArgMin<T>(this T[][] matrix)
            where T : IComparable<T>
        {
            Tuple<int, int> index;
            Min(matrix, out index);
            return index;
        }

        /// <summary>
        ///     Gets the index of the minimum element in a matrix across a given dimension.
        /// </summary>
        public static int[] ArgMin<T>(this T[][] matrix, int dimension)
            where T : IComparable<T>
        {
            var s = GetLength(matrix, dimension);
            var values = new T[s];
            var indices = new int[s];
            Min(matrix, dimension, indices, values);
            return indices;
        }

        /// <summary>
        ///     Gets the index of the minimum element in a matrix across a given dimension.
        /// </summary>
        public static int[] ArgMin<T>(this T[][] matrix, int dimension, int[] result)
            where T : IComparable<T>
        {
            var s = GetLength(matrix, dimension);
            var values = new T[s];
            Min(matrix, dimension, result, values);
            return result;
        }

        #endregion
        #region Matrix Min/Max

        /// <summary>
        ///     Gets the maximum value of a matrix.
        /// </summary>
        public static T Max<T>(this T[][] matrix)
            where T : IComparable<T>
        {
            var max = matrix[0][0];
            for (var i = 0; i < matrix.Length; i++)
                for (var j = 0; j < matrix[i].Length; j++)
                    if (matrix[i][j].CompareTo(max) > 0)
                        max = matrix[i][j];

            return max;
        }

        /// <summary>
        ///     Gets the minimum value of a matrix.
        /// </summary>
        public static T Min<T>(this T[][] matrix)
            where T : IComparable<T>
        {
            Tuple<int, int> index;
            return Min(matrix, out index);
        }
        /// <summary>
        ///     Gets the minimum values across one dimension of a matrix.
        /// </summary>
        public static T[] Min<T>(this T[][] matrix, int dimension)
            where T : IComparable<T>
        {
            var s = GetLength(matrix, dimension);
            var result = new T[s];
            var indices = new int[s];
            return Min(matrix, dimension, indices, result);
        }

        /// <summary>
        ///     Gets the maximum values across one dimension of a matrix.
        /// </summary>
        public static T[] Max<T>(this T[][] matrix, int dimension)
            where T : IComparable<T>
        {
            var s = GetLength(matrix, dimension);
            var result = new T[s];
            var indices = new int[s];
            return Max(matrix, dimension, indices, result);
        }

        /// <summary>
        ///     Gets the maximum values across one dimension of a matrix.
        /// </summary>
        public static T[] Max<T>(this T[][] matrix, int dimension, T[] result)
            where T : IComparable<T>
        {
            var s = GetLength(matrix, dimension);
            var indices = new int[s];
            return Max(matrix, dimension, indices, result);
        }

        /// <summary>
        ///     Gets the minimum values across one dimension of a matrix.
        /// </summary>
        public static T[] Min<T>(this T[][] matrix, int dimension, T[] result)
            where T : IComparable<T>
        {
            var s = GetLength(matrix, dimension);
            var indices = new int[s];
            return Min(matrix, dimension, indices, result);
        }
        /// <summary>
        ///     Gets the minimum values across one dimension of a matrix.
        /// </summary>
        public static T[] Min<T>(this T[][] matrix, int dimension, out int[] indices)
            where T : IComparable<T>
        {
            var s = GetLength(matrix, dimension);
            var result = new T[s];
            indices = new int[s];
            return Min(matrix, dimension, indices, result);
        }

        /// <summary>
        ///     Gets the maximum values across one dimension of a matrix.
        /// </summary>
        public static T[] Max<T>(this T[][] matrix, int dimension, out int[] indices)
            where T : IComparable<T>
        {
            var s = GetLength(matrix, dimension);
            var result = new T[s];
            indices = new int[s];
            return Max(matrix, dimension, indices, result);
        }

        /// <summary>
        ///     Gets the maximum values across one dimension of a matrix.
        /// </summary>
        public static T[] Max<T>(this T[][] matrix, int dimension, out int[] indices, T[] result)
            where T : IComparable<T>
        {
            var s = GetLength(matrix, dimension);
            indices = new int[s];
            return Max(matrix, dimension, indices, result);
        }

        /// <summary>
        ///     Gets the minimum values across one dimension of a matrix.
        /// </summary>
        public static T[] Min<T>(this T[][] matrix, int dimension, out int[] indices, T[] result)
            where T : IComparable<T>
        {
            var s = GetLength(matrix, dimension);
            indices = new int[s];
            return Min(matrix, dimension, indices, result);
        }

        #endregion
        #region Core implementations

        /// <summary>
        ///     Gets the maximum value of a matrix.
        /// </summary>
        public static T Max<T>(this T[][] matrix, out Tuple<int, int> imax)
            where T : IComparable<T>
        {
            var max = matrix[0][0];
            imax = Tuple.Create(0, 0);

            for (var i = 0; i < matrix.Length; i++)
                for (var j = 0; j < matrix[i].Length; j++)
                    if (matrix[i][j].CompareTo(max) > 0)
                    {
                        max = matrix[i][j];
                        imax = Tuple.Create(i, j);
                    }

            return max;
        }

        /// <summary>
        ///     Gets the minimum value of a matrix.
        /// </summary>
        public static T Min<T>(this T[][] matrix, out Tuple<int, int> imin)
            where T : IComparable<T>
        {
            var min = matrix[0][0];
            imin = Tuple.Create(0, 0);

            for (var i = 0; i < matrix.Length; i++)
                for (var j = 0; j < matrix[i].Length; j++)
                    if (matrix[i][j].CompareTo(min) < 0)
                    {
                        min = matrix[i][j];
                        imin = Tuple.Create(i, j);
                    }

            return min;
        }

        /// <summary>
        ///     Gets the maximum values across one dimension of a matrix.
        /// </summary>
        public static T[] Max<T>(this T[][] matrix, int dimension, int[] indices, T[] result)
            where T : IComparable<T>
        {
            if (dimension == 1) // Search down columns
            {
                matrix.GetColumn(0, result);
                for (var j = 0; j < matrix.Length; j++)
                    for (var i = 0; i < matrix[j].Length; i++)
                        if (matrix[j][i].CompareTo(result[j]) > 0)
                        {
                            result[j] = matrix[j][i];
                            indices[j] = i;
                        }
            }
            else
            {
                matrix.GetRow(0, result);
                for (var j = 0; j < result.Length; j++)
                    for (var i = 0; i < matrix.Length; i++)
                        if (matrix[i][j].CompareTo(result[j]) > 0)
                        {
                            result[j] = matrix[i][j];
                            indices[j] = i;
                        }
            }

            return result;
        }
        /// <summary>
        ///     Gets the minimum values across one dimension of a matrix.
        /// </summary>
        public static T[] Min<T>(this T[][] matrix, int dimension, int[] indices, T[] result)
            where T : IComparable<T>
        {
            if (dimension == 1) // Search down columns
            {
                matrix.GetColumn(0, result);
                for (var j = 0; j < matrix.Length; j++)
                    for (var i = 0; i < matrix[j].Length; i++)
                        if (matrix[j][i].CompareTo(result[j]) < 0)
                        {
                            result[j] = matrix[j][i];
                            indices[j] = i;
                        }
            }
            else
            {
                matrix.GetRow(0, result);
                for (var j = 0; j < result.Length; j++)
                    for (var i = 0; i < matrix.Length; i++)
                        if (matrix[i][j].CompareTo(result[j]) < 0)
                        {
                            result[j] = matrix[i][j];
                            indices[j] = i;
                        }
            }

            return result;
        }

        #endregion
    }
}