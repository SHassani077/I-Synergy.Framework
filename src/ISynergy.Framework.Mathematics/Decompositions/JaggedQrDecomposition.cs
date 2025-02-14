﻿using ISynergy.Framework.Mathematics.Common;
using ISynergy.Framework.Mathematics.Decompositions.Base;
using System;

namespace ISynergy.Framework.Mathematics.Decompositions
{
    /// <summary>
    ///     QR decomposition for a rectangular matrix.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         For an m-by-n matrix <c>A</c> with <c>m >= n</c>, the QR decomposition
    ///         is an m-by-n orthogonal matrix <c>Q</c> and an n-by-n upper triangular
    ///         matrix <c>R</c> so that <c>A = Q * R</c>.
    ///     </para>
    ///     <para>
    ///         The QR decomposition always exists, even if the matrix does not have
    ///         full rank, so the constructor will never fail. The primary use of the
    ///         QR decomposition is in the least squares solution of nonsquare systems
    ///         of simultaneous linear equations.
    ///         This will fail if <see cref="FullRank" /> returns <see langword="false" />.
    ///     </para>
    /// </remarks>
    public sealed class JaggedQrDecomposition : ICloneable, ISolverArrayDecomposition<double>
    {
        private bool economy;

        // cache for lazy evaluation
        private bool? fullRank;
        private int m;
        private int n;
        private double[][] orthogonalFactor;
        private int p;

        private double[][] qr;
        private double[][] upperTriangularFactor;

        /// <summary>Constructs a QR decomposition.</summary>
        /// <param name="value">The matrix A to be decomposed.</param>
        /// <param name="transpose">
        ///     True if the decomposition should be performed on
        ///     the transpose of A rather than A itself, false otherwise. Default is false.
        /// </param>
        /// <param name="inPlace">
        ///     True if the decomposition should be done in place,
        ///     overriding the given matrix <paramref name="value" />. Default is false.
        /// </param>
        /// <param name="economy">
        ///     True to perform the economy decomposition, where only
        ///     .the information needed to solve linear systems is computed. If set to false,
        ///     the full QR decomposition will be computed.
        /// </param>
        public JaggedQrDecomposition(double[][] value, bool transpose = false,
            bool economy = true, bool inPlace = false)
        {
            if (value == null) throw new ArgumentNullException("value", "Matrix cannot be null.");

            if (!transpose && value.Length < value[0].Length ||
                transpose && value[0].Length < value.Length)
                throw new ArgumentException("Matrix has more columns than rows.", "value");

            // https://www.inf.ethz.ch/personal/gander/papers/qrneu.pdf

            if (transpose)
            {
                p = value.Rows();

                if (economy)
                    // Compute the faster, economy-sized QR decomposition 
                    qr = value.Transpose(inPlace);
                else
                    // Create room to store the full decomposition
                    qr = Jagged.Create(value.Columns(), value.Columns(), value, true);
            }
            else
            {
                p = value.Columns();

                if (economy)
                    // Compute the faster, economy-sized QR decomposition 
                    qr = inPlace ? value : value.Copy();
                else
                    // Create room to store the full decomposition
                    qr = Jagged.Create(value.Rows(), value.Rows(), value, false);
            }

            this.economy = economy;
            n = qr.Rows();
            m = qr.Columns();
            Diagonal = new double[m];

            for (var k = 0; k < m; k++)
            {
                // Compute 2-norm of k-th column without under/overflow.
                double nrm = 0;
                for (var i = k; i < qr.Length; i++)
                    nrm = Tools.Hypotenuse(nrm, qr[i][k]);

                if (nrm != 0)
                {
                    // Form k-th Householder vector.
                    if (qr[k][k] < 0)
                        nrm = -nrm;

                    for (var i = k; i < qr.Length; i++)
                        qr[i][k] /= nrm;

                    qr[k][k] += 1;

                    // Apply transformation to remaining columns.
                    for (var j = k + 1; j < m; j++)
                    {
                        double s = 0;
                        for (var i = k; i < qr.Length; i++)
                            s += qr[i][k] * qr[i][j];

                        s = -s / qr[k][k];
                        for (var i = k; i < qr.Length; i++)
                            qr[i][j] += s * qr[i][k];
                    }
                }

                Diagonal[k] = -nrm;
            }
        }

        /// <summary>Shows if the matrix <c>A</c> is of full rank.</summary>
        /// <value>The value is <see langword="true" /> if <c>R</c>, and hence <c>A</c>, has full rank.</value>
        public bool FullRank
        {
            get
            {
                if (fullRank.HasValue)
                    return fullRank.Value;

                for (var i = 0; i < p; i++)
                    if (Diagonal[i] == 0)
                        return (bool)(fullRank = false);

                return (bool)(fullRank = true);
            }
        }

        /// <summary>Returns the upper triangular factor <c>R</c>.</summary>
        public double[][] UpperTriangularFactor
        {
            get
            {
                if (upperTriangularFactor != null)
                    return upperTriangularFactor;

                var rows = economy ? m : n;
                var x = Jagged.Zeros<double>(rows, p);

                for (var i = 0; i < x.Length; i++)
                    for (var j = 0; j < x[i].Length; j++)
                        if (i < j)
                            x[i][j] = qr[i][j];
                        else if (i == j)
                            x[i][j] = Diagonal[i];

                return upperTriangularFactor = x;
            }
        }

        /// <summary>
        ///     Returns the orthogonal factor <c>Q</c>.
        /// </summary>
        public double[][] OrthogonalFactor
        {
            get
            {
                if (orthogonalFactor != null)
                    return orthogonalFactor;

                var cols = economy ? m : n;
                var x = Jagged.Zeros<double>(n, cols);

                for (var k = cols - 1; k >= 0; k--)
                {
                    for (var i = 0; i < n; i++)
                        x[i][k] = 0;

                    x[k][k] = 1;
                    for (var j = k; j < cols; j++)
                        if (qr[k][k] != 0)
                        {
                            double s = 0;
                            for (var i = k; i < qr.Length; i++)
                                s += qr[i][k] * x[i][j];

                            s = -s / qr[k][k];
                            for (var i = k; i < qr.Length; i++)
                                x[i][j] += s * qr[i][k];
                        }
                }

                return orthogonalFactor = x;
            }
        }

        /// <summary>Returns the diagonal of <c>R</c>.</summary>
        public double[] Diagonal { get; private set; }

        /// <summary>Least squares solution of <c>A * X = B</c></summary>
        /// <param name="value">Right-hand-side matrix with as many rows as <c>A</c> and any number of columns.</param>
        /// <returns>A matrix that minimized the two norm of <c>Q * R * X - B</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix row dimensions must be the same.</exception>
        /// <exception cref="T:System.InvalidOperationException">Matrix is rank deficient.</exception>
        public double[][] Solve(double[][] value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Matrix cannot be null.");

            if (value.Length != n)
                throw new ArgumentException("Matrix row dimensions must agree.");

            if (!FullRank)
                throw new InvalidOperationException("Matrix is rank deficient.");

            // Copy right hand side
            var count = value.Columns();
            var X = value.Copy();

            // Compute Y = transpose(Q)*B
            for (var k = 0; k < p; k++)
                for (var j = 0; j < count; j++)
                {
                    double s = 0;
                    for (var i = k; i < qr.Length; i++)
                        s += qr[i][k] * X[i][j];

                    s = -s / qr[k][k];
                    for (var i = k; i < qr.Length; i++)
                        X[i][j] += s * qr[i][k];
                }

            // Solve R*X = Y;
            for (var k = p - 1; k >= 0; k--)
            {
                for (var j = 0; j < X[k].Length; j++)
                    X[k][j] /= Diagonal[k];

                for (var i = 0; i < k; i++)
                    for (var j = 0; j < X[i].Length; j++)
                        X[i][j] -= X[k][j] * qr[i][k];
            }

            return Jagged.Create(p, count, X, false);
        }

        /// <summary>Least squares solution of <c>A * X = B</c></summary>
        /// <param name="value">Right-hand-side matrix with as many rows as <c>A</c> and any number of columns.</param>
        /// <returns>A matrix that minimized the two norm of <c>Q * R * X - B</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix row dimensions must be the same.</exception>
        /// <exception cref="T:System.InvalidOperationException">Matrix is rank deficient.</exception>
        public double[] Solve(double[] value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            if (value.Length != qr.Length)
                throw new ArgumentException("Matrix row dimensions must agree.");

            if (!FullRank)
                throw new InvalidOperationException("Matrix is rank deficient.");

            // Copy right hand side
            var X = value.Copy();

            // Compute Y = transpose(Q)*B
            for (var k = 0; k < p; k++)
            {
                double s = 0;
                for (var i = k; i < qr.Length; i++)
                    s += qr[i][k] * X[i];

                s = -s / qr[k][k];
                for (var i = k; i < qr.Length; i++)
                    X[i] += s * qr[i][k];
            }

            // Solve R*X = Y;
            for (var k = p - 1; k >= 0; k--)
            {
                X[k] /= Diagonal[k];

                for (var i = 0; i < k; i++)
                    X[i] -= X[k] * qr[i][k];
            }

            return X.First(p);
        }

        /// <summary>
        ///     Solves a set of equation systems of type <c>A * X = B</c> where B is a diagonal matrix.
        /// </summary>
        /// <param name="diagonal">Diagonal fo the right hand side matrix with as many rows as <c>A</c>.</param>
        /// <returns>Matrix <c>X</c> so that <c>L * U * X = B</c>.</returns>
        public double[][] SolveForDiagonal(double[] diagonal)
        {
            if (diagonal == null)
                throw new ArgumentNullException("diagonal");

            return Solve(Jagged.Diagonal(diagonal));
        }

        /// <summary>Least squares solution of <c>A * X = I</c></summary>
        public double[][] Inverse()
        {
            if (!FullRank)
                throw new InvalidOperationException("Matrix is rank deficient.");

            return Solve(Jagged.Diagonal(n, n, (double)1));
        }

        /// <summary>
        ///     Reverses the decomposition, reconstructing the original matrix <c>X</c>.
        /// </summary>
        public double[][] Reverse()
        {
            return OrthogonalFactor.Dot(UpperTriangularFactor);
        }

        /// <summary>
        ///     Computes <c>(Xt * X)^1</c> (the inverse of the covariance matrix). This
        ///     matrix can be used to determine standard errors for the coefficients when
        ///     solving a linear set of equations through any of the <see cref="Solve(Double[][])" />
        ///     methods.
        /// </summary>
        public double[][] GetInformationMatrix()
        {
            var X = Reverse();
            return X.TransposeAndDot(X).Inverse();
        }

        /// <summary>Least squares solution of <c>X * A = B</c></summary>
        /// <param name="value">Right-hand-side matrix with as many columns as <c>A</c> and any number of rows.</param>
        /// <returns>A matrix that minimized the two norm of <c>X * Q * R - B</c>.</returns>
        /// <exception cref="T:System.ArgumentException">Matrix column dimensions must be the same.</exception>
        /// <exception cref="T:System.InvalidOperationException">Matrix is rank deficient.</exception>
        public double[][] SolveTranspose(double[][] value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Matrix cannot be null.");

            if (value.Columns() != qr.Length)
                throw new ArgumentException("Matrix row dimensions must agree.");

            if (!FullRank)
                throw new InvalidOperationException("Matrix is rank deficient.");

            // Copy right hand side
            var count = value.Length;
            var X = value.Transpose();

            // Compute Y = transpose(Q)*B
            for (var k = 0; k < p; k++)
                for (var j = 0; j < count; j++)
                {
                    double s = 0;
                    for (var i = k; i < n; i++)
                        s += qr[i][k] * X[i][j];

                    s = -s / qr[k][k];
                    for (var i = k; i < n; i++)
                        X[i][j] += s * qr[i][k];
                }

            // Solve R*X = Y;
            for (var k = p - 1; k >= 0; k--)
            {
                for (var j = 0; j < count; j++)
                    X[k][j] /= Diagonal[k];

                for (var i = 0; i < k; i++)
                    for (var j = 0; j < count; j++)
                        X[i][j] -= X[k][j] * qr[i][k];
            }

            return Jagged.Create(count, p, X, true);
        }
        #region ICloneable Members

        private JaggedQrDecomposition()
        {
        }

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var clone = new JaggedQrDecomposition();
            clone.qr = qr.Copy();
            clone.Diagonal = Diagonal.Copy();
            clone.n = n;
            clone.p = p;
            clone.m = m;
            clone.economy = economy;
            return clone;
        }

        #endregion
    }
}