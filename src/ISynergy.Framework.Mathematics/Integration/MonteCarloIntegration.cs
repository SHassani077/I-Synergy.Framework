﻿using ISynergy.Framework.Core.Ranges;
using System;

namespace ISynergy.Framework.Mathematics.Integration
{
    /// <summary>
    ///     Monte Carlo method for multi-dimensional integration.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         In mathematics, Monte Carlo integration is a technique for numerical
    ///         integration using random numbers. It is a particular Monte Carlo method
    ///         that numerically computes a definite integral. While other algorithms
    ///         usually evaluate the integrand at a regular grid, Monte Carlo randomly
    ///         choose points at which the integrand is evaluated. This method is
    ///         particularly useful for higher-dimensional integrals. There are different
    ///         methods to perform a Monte Carlo integration, such as uniform sampling,
    ///         stratified sampling and importance sampling.
    ///     </para>
    ///     <para>
    ///         References:
    ///         <list type="bullet">
    ///             <item>
    ///                 <description>
    ///                     <a href="http://en.wikipedia.org/wiki/Monte_Carlo_integration">
    ///                         Wikipedia, The Free Encyclopedia. Monte Carlo integration. Available on:
    ///                         http://en.wikipedia.org/wiki/Monte_Carlo_integration
    ///                     </a>
    ///                 </description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <para>
    ///         A common Monte-Carlo integration example is to compute the value of Pi. This is the
    ///         same example given in Wikipedia's page for Monte-Carlo Integration, available at
    ///         https://en.wikipedia.org/wiki/Monte_Carlo_integration#Example
    ///     </para>
    ///     <code>
    /// // Define a function H that tells whether two points 
    /// // are inside a unit circle (a circle of radius one):
    /// //
    /// Func&lt;double, double, double> H = 
    ///     (x, y) => (x * x + y * y &lt;= 1) ? 1 : 0;
    /// 
    /// // We will check how many points in the square (-1,-1), (-1,+1), 
    /// // (+1, -1), (+1, +1) fall into the circle defined by function H.
    /// //
    /// double[] from = { -1, -1 };
    /// double[] to   = { +1, +1 };
    /// 
    /// int samples = 100000;
    /// 
    /// // Integrate it! 
    /// double area = MonteCarloIntegration.Integrate(x => H(x[0], x[1]), from, to, samples);
    /// 
    /// // Output should be approximately 3.14.
    /// </code>
    /// </example>
    /// <seealso cref="NonAdaptiveGaussKronrod" />
    /// <seealso cref="InfiniteAdaptiveGaussKronrod" />
    public class MonteCarloIntegration : INumericalIntegration, IMultidimensionalIntegration
    {
        private int count;
        private double sum;
        private double sum2;

        /// <summary>
        ///     Constructs a new <see cref="MonteCarloIntegration">Monte Carlo integration method</see>.
        /// </summary>
        /// <param name="function">The function to be integrated.</param>
        /// <param name="parameters">The number of parameters expected by the <paramref name="function" />.</param>
        public MonteCarloIntegration(int parameters, Func<double[], double> function)
            : this(parameters)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            Function = function;
        }

        /// <summary>
        ///     Constructs a new <see cref="MonteCarloIntegration">Monte Carlo integration method</see>.
        /// </summary>
        /// <param name="parameters">The number of parameters expected by the integrand.</param>
        public MonteCarloIntegration(int parameters)
        {
            if (parameters <= 0)
                throw new ArgumentOutOfRangeException("parameters",
                    "Number of parameters must be higher than zero.");

            NumberOfParameters = parameters;
            Range = new NumericRange[parameters];
            Random = new System.Random(Framework.Mathematics.Random.Generator.Random.Next());

            for (var i = 0; i < Range.Length; i++)
                Range[i].Max = 1;

            Iterations = 1000;
            Reset();
        }

        /// <summary>
        ///     Gets or sets the random generator algorithm to be used within
        ///     this <see cref="MonteCarloIntegration">Monte Carlo method</see>.
        /// </summary>
        public System.Random Random { get; set; }

        /// <summary>
        ///     Gets the integration error for the
        ///     computed <see cref="Area" /> value.
        /// </summary>
        public double Error { get; set; }

        /// <summary>
        ///     Gets or sets the number of random samples
        ///     (iterations) generated by the algorithm.
        /// </summary>
        public int Iterations { get; set; }

        /// <summary>
        ///     Gets the number of parameters expected by
        ///     the <see cref="Function" /> to be integrated.
        /// </summary>
        public int NumberOfParameters { get; }

        /// <summary>
        ///     Gets or sets the range of each input variable
        ///     under which the integral must be computed.
        /// </summary>
        public NumericRange[] Range { get; private set; }

        /// <summary>
        ///     Gets or sets the multidimensional function
        ///     whose integral should be computed.
        /// </summary>
        public Func<double[], double> Function { get; set; }

        /// <summary>
        ///     Gets the numerically computed result of the
        ///     definite integral for the specified function.
        /// </summary>
        public double Area { get; set; }

        /// <summary>
        ///     Computes the area of the function under the selected <see cref="Range" />.
        ///     The computed value will be available at this object's <see cref="Area" />.
        /// </summary>
        /// <returns>
        ///     True if the integration method succeeds, false otherwise.
        /// </returns>
        public bool Compute()
        {
            var sample = new double[NumberOfParameters];

            for (var j = 0; j < Iterations; j++)
            {
                for (var i = 0; i < sample.Length; i++)
                    sample[i] = Random.NextDouble() * Range[i].Length + Range[i].Min;

                var f = Function(sample);

                count++;
                sum += f;
                sum2 += f * f;
            }

            double volume = 1;
            for (var i = 0; i < Range.Length; i++)
                volume *= Range[i].Length;

            var avg = sum / count;
            var avg2 = sum2 / count;

            Area = volume * avg;
            Error = volume * Math.Sqrt((avg2 - avg * avg) / count);

            return true;
        }
        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        public object Clone()
        {
            var clone = new MonteCarloIntegration(NumberOfParameters, Function);

            clone.Iterations = Iterations;
            clone.Range = (NumericRange[])Range.Clone();

            return clone;
        }

        /// <summary>
        ///     Manually resets the previously computed area and error
        ///     estimates, so the integral can be computed from scratch
        ///     without reusing previous computations.
        /// </summary>
        public void Reset()
        {
            sum = 0;
            sum2 = 0;
            count = 0;
            Area = 0;
            Error = double.PositiveInfinity;
        }

        /// <summary>
        ///     Computes the area of the function under the selected <see cref="Range" />.
        ///     The computed value will be available at this object's <see cref="Area" />.
        /// </summary>
        /// <returns>
        ///     True if the integration method succeeds, false otherwise.
        /// </returns>
        public static double Integrate(Func<double[], double> func, double[] a, double[] b, int samples)
        {
            var mc = new MonteCarloIntegration(a.Length, func);
            for (var i = 0; i < a.Length; i++)
                mc.Range[i] = new NumericRange(a[i], b[i]);
            mc.Iterations = samples;
            mc.Compute();
            return mc.Area;
        }

        /// <summary>
        ///     Computes the area of the function under the selected <see cref="Range" />.
        ///     The computed value will be available at this object's <see cref="Area" />.
        /// </summary>
        /// <returns>
        ///     True if the integration method succeeds, false otherwise.
        /// </returns>
        public static double Integrate(Func<double[], double> func, double[] a, double[] b)
        {
            var mc = new MonteCarloIntegration(a.Length, func);
            for (var i = 0; i < a.Length; i++)
                mc.Range[i] = new NumericRange(a[i], b[i]);
            mc.Compute();
            return mc.Area;
        }

        /// <summary>
        ///     Computes the area under the integral for the given function, in the
        ///     given integration interval, using a Monte Carlo integration algorithm.
        /// </summary>
        /// <param name="func">The unidimensional function whose integral should be computed.</param>
        /// <param name="a">The beginning of the integration interval.</param>
        /// <param name="b">The ending of the integration interval.</param>
        /// <param name="samples">The number of points that should be sampled.</param>
        /// <returns>The integral's value in the current interval.</returns>
        public static double Integrate(Func<double, double> func, double a, double b, int samples)
        {
            var volume = b - a;

            var count = 0;
            double sum = 0;

            var random = new System.Random(Framework.Mathematics.Random.Generator.Random.Next());

            for (count = 0; count < samples; count++)
            {
                var u = random.Next() * (b - a) + a;

                var f = func(u);

                count++;
                sum += f;
            }

            var avg = sum / count;

            return volume * avg;
        }
    }
}