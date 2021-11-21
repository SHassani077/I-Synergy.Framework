﻿namespace ISynergy.Framework.Mathematics.Distances
{
    using ISynergy.Framework.Mathematics.Distances.Base;
    using System;

    /// <summary>
    ///   The Minkowski distance is a metric in a normed vector space which can be 
    ///   considered as a generalization of both the <see cref="Euclidean">Euclidean 
    ///   distance</see> and the <see cref="Manhattan">Manhattan distance</see>.
    /// </summary>
    /// 
    /// <remarks>
    /// <para>
    ///   The framework distinguishes between metrics and distances by using different
    ///   types for them. This makes it possible to let the compiler figure out logic
    ///   problems such as the specification of a non-metric for a method that requires
    ///   a proper metric (i.e. that respects the triangle inequality).</para>
    ///   
    /// <para>
    ///   The objective of this technique is to make it harder to make some mistakes.
    ///   However, it is possible to bypass this mechanism by using the named constructors
    ///   such as <see cref="Nonmetric"/> to create distances implementing the <see cref="IMetric{T}"/>
    ///   interface that are not really metrics. Use at your own risk.</para>
    /// </remarks>
    /// 
    [Serializable]
    public struct Minkowski : IMetric<double[]>, IMetric<int[]>, ICloneable
    {
        private double p;

        /// <summary>
        ///   Gets the order <c>p</c> of this Minkowski distance.
        /// </summary>
        /// 
        public double Order { get { return p; } }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Minkowski"/> class.
        /// </summary>
        /// 
        /// <param name="p">The Minkowski order <c>p</c>.</param>
        /// 
        /// <exception cref="System.ArgumentOutOfRangeException">The Minkowski distance is not a metric for p &lt; 1.</exception>
        /// 
        public Minkowski(double p)
        {
            if (p < 1)
                throw new ArgumentOutOfRangeException("The Minkowski distance is not a metric for p < 1.");

            this.p = p;
        }

        /// <summary>
        ///   Computes the distance <c>d(x,y)</c> between points
        ///   <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// 
        /// <param name="x">The first point <c>x</c>.</param>
        /// <param name="y">The second point <c>y</c>.</param>
        /// 
        /// <returns>
        ///   A double-precision value representing the distance <c>d(x,y)</c>
        ///   between <paramref name="x"/> and <paramref name="y"/> according 
        ///   to the distance function implemented by this class.
        /// </returns>
        /// 
        public double Distance(int[] x, int[] y)
        {
            double sum = 0;
            for (var i = 0; i < x.Length; i++)
                sum += Math.Pow(Math.Abs(x[i] - y[i]), p);
            return Math.Pow(sum, 1 / p);
        }

        /// <summary>
        ///   Computes the distance <c>d(x,y)</c> between points
        ///   <paramref name="x"/> and <paramref name="y"/>.
        /// </summary>
        /// 
        /// <param name="x">The first point <c>x</c>.</param>
        /// <param name="y">The second point <c>y</c>.</param>
        /// 
        /// <returns>
        ///   A double-precision value representing the distance <c>d(x,y)</c>
        ///   between <paramref name="x"/> and <paramref name="y"/> according 
        ///   to the distance function implemented by this class.
        /// </returns>
        /// 
        public double Distance(double[] x, double[] y)
        {
            double sum = 0;
            for (var i = 0; i < x.Length; i++)
                sum += Math.Pow(Math.Abs(x[i] - y[i]), p);
            return Math.Pow(sum, 1 / p);
        }

        /// <summary>
        ///   Creates a non-metric Minkowski distance, bypassing
        ///   argument checking. Use at your own risk.
        /// </summary>
        /// 
        /// <param name="p">The Minkowski order <c>p</c>.</param>
        /// 
        /// <returns>A Minkowski object implementing a Minkowski distance
        ///   that is not necessarily a metric. Use at your own risk.</returns>
        /// 
        public static Minkowski Nonmetric(double p)
        {
            var minkowski = new Minkowski(1);
            minkowski.p = p;
            return minkowski;
        }

        /// <summary>
        ///   Gets the <see cref="Manhattan"/> distance as a special 
        ///   case of the <see cref="Minkowski"/> distance.
        /// </summary>
        /// 
        public static readonly Minkowski Manhattan = new Minkowski(1);

        /// <summary>
        ///   Gets the <see cref="Euclidean"/> distance as a special 
        ///   case of the <see cref="Minkowski"/> distance.
        /// </summary>
        /// 
        public static readonly Minkowski Euclidean = new Minkowski(2);

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new Minkowski(p);
        }
    }
}
