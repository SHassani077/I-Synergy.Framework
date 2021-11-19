﻿namespace ISynergy.Framework.Mathematics.Optimization.Unconstrained
{

    /* Unmerged change from project 'ISynergy.Framework.Mathematics (net5.0)'
    Before:
        using System;
    After:
        using ISynergy.Framework.Mathematics.Optimization.Unconstrained;
        using System;
    */

    /* Unmerged change from project 'ISynergy.Framework.Mathematics (net6.0)'
    Before:
        using System;
    After:
        using ISynergy.Framework.Mathematics.Optimization.Unconstrained;
        using System;
    */
    using System;

    /// <summary>
    ///   Optimization progress event arguments.
    /// </summary>
    public class OptimizationProgressEventArgs : EventArgs
    {
        /// <summary>
        ///   Gets the current iteration of the method.
        /// </summary>
        /// 
        public int Iteration { get; private set; }

        /// <summary>
        ///   Gets the number of function evaluations performed.
        /// </summary>
        /// 
        public int Evaluations { get; private set; }

        /// <summary>
        ///   Gets the current gradient of the function being optimized.
        /// </summary>
        /// 
        public double[] Gradient { get; private set; }

        /// <summary>
        ///   Gets the norm of the current <see cref="Gradient"/>.
        /// </summary>
        /// 
        public double GradientNorm { get; private set; }

        /// <summary>
        ///   Gets the current solution parameters for the problem.
        /// </summary>
        /// 
        public double[] Solution { get; private set; }

        /// <summary>
        ///   Gets the norm of the current <see cref="Solution"/>.
        /// </summary>
        /// 
        public double SolutionNorm { get; private set; }

        /// <summary>
        ///   Gets the value of the function to be optimized
        ///   at the current proposed <see cref="Solution"/>.
        /// </summary>
        /// 
        public double Value { get; private set; }

        /// <summary>
        ///   Gets the current step size.
        /// </summary>
        /// 
        public double Step { get; private set; }
        /// <summary>
        ///   Gets or sets a value indicating whether the
        ///   optimization process is about to terminate.
        /// </summary>
        /// 
        /// <value><c>true</c> if finished; otherwise, <c>false</c>.</value>
        /// 
        public bool Finished { get; private set; }

        /// <summary>
        ///   An user-defined value associated with this object.
        /// </summary>
        /// 
        public object Tag { get; set; }

        /// <summary>
        ///   Initializes a new instance of the <see cref="OptimizationProgressEventArgs"/> class.
        /// </summary>
        /// 
        /// <param name="iteration">The current iteration of the optimization method.</param>
        /// <param name="evaluations">The number of function evaluations performed.</param>
        /// <param name="gradient">The current gradient of the function.</param>
        /// <param name="gnorm">The norm of the current gradient</param>
        /// <param name="xnorm">The norm of the current parameter vector.</param>
        /// <param name="solution">The current solution parameters.</param>
        /// <param name="value">The value of the function evaluated at the current solution.</param>
        /// <param name="stp">The current step size.</param>
        /// <param name="finished"><c>True</c> if the method is about to terminate, <c>false</c> otherwise.</param>
        /// 
        public OptimizationProgressEventArgs(
            int iteration, int evaluations,
            double[] gradient, double gnorm,
            double[] solution, double xnorm,
            double value, double stp, bool finished)
        {
            if (gradient != null)
                Gradient = (double[])gradient.Clone();

            if (solution != null)
                Solution = (double[])solution.Clone();

            Value = value;
            GradientNorm = gnorm;
            SolutionNorm = xnorm;

            Iteration = iteration;
            Evaluations = evaluations;

            Finished = finished;
            Step = stp;
        }
    }
}
