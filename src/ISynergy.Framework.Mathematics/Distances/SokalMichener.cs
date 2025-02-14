﻿namespace ISynergy.Framework.Mathematics.Distances
{
    using ISynergy.Framework.Mathematics.Distances.Base;
    using System;

    /// <summary>
    ///   Sokal-Michener dissimilarity.
    /// </summary>
    /// 
    [Serializable]
    public struct SokalMichener : IDistance<double[]>, IDistance<int[]>, ICloneable
    {
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
            int tf = 0;
            int ft = 0;
            int tt = 0;
            int ff = 0;

            for (var i = 0; i < x.Length; i++)
            {
                if (x[i] == 1 && y[i] == 0) tf++;
                if (x[i] == 0 && y[i] == 1) ft++;
                if (x[i] == 1 && y[i] == 1) tt++;
                if (x[i] == 0 && y[i] == 0) ff++;
            }

            int r = 2 * (tf + ft);
            return r / (double)(ff + tt + r);
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
            int tf = 0;
            int ft = 0;
            int tt = 0;
            int ff = 0;

            for (var i = 0; i < x.Length; i++)
            {
                if (x[i] == 1 && y[i] == 0) tf++;
                if (x[i] == 0 && y[i] == 1) ft++;
                if (x[i] == 1 && y[i] == 1) tt++;
                if (x[i] == 0 && y[i] == 0) ff++;
            }

            int r = 2 * (tf + ft);
            return r / (double)(ff + tt + r);
        }
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>A new object that is a copy of this instance.</returns>
        public object Clone()
        {
            return new SokalMichener();
        }
    }
}
