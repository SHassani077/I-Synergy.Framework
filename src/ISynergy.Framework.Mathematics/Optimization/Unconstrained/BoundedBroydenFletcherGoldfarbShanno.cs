﻿using ISynergy.Framework.Core.Abstractions.Async;
using ISynergy.Framework.Mathematics.Exceptions;
using ISynergy.Framework.Mathematics.Optimization.Base;
using ISynergy.Framework.Mathematics.Optimization.Unconstrained;
using System;
using System.ComponentModel;

namespace ISynergy.Framework.Mathematics.Optimization
{
    /// <summary>
    ///     Status codes for the <see cref="BoundedBroydenFletcherGoldfarbShanno" />
    ///     function optimizer.
    /// </summary>
    public enum BoundedBroydenFletcherGoldfarbShannoStatus
    {
        /// <summary>
        ///     The optimization stopped before convergence; maximum
        ///     number of iterations could have been reached.
        /// </summary>
        Stop,

        /// <summary>
        ///     Maximum number of iterations was reached.
        /// </summary>
        MaximumIterations,

        /// <summary>
        ///     The function output converged to a static
        ///     value within the desired precision.
        /// </summary>
        FunctionConvergence,

        /// <summary>
        ///     The function gradient converged to a minimum
        ///     value within the desired precision.
        /// </summary>
        GradientConvergence,

        /// <summary>
        ///     The inner line search function failed. This could be an indication
        ///     that there might be something wrong with the gradient function.
        /// </summary>
        [Description("ABNORMAL_TERMINATION_IN_LNSRCH")]
        LineSearchFailed = -1
    }

    /// <summary>
    ///     Limited-memory Broyden–Fletcher–Goldfarb–Shanno (L-BFGS) optimization method.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The L-BFGS algorithm is a member of the broad family of quasi-Newton optimization
    ///         methods. L-BFGS stands for 'Limited memory BFGS'. Indeed, L-BFGS uses a limited
    ///         memory variation of the Broyden–Fletcher–Goldfarb–Shanno (BFGS) update to approximate
    ///         the inverse Hessian matrix (denoted by Hk). Unlike the original BFGS method which
    ///         stores a dense  approximation, L-BFGS stores only a few vectors that represent the
    ///         approximation implicitly. Due to its moderate memory requirement, L-BFGS method is
    ///         particularly well suited for optimization problems with a large number of variables.
    ///     </para>
    ///     <para>
    ///         L-BFGS never explicitly forms or stores Hk. Instead, it maintains a history of the past
    ///         <c>m</c> updates of the position <c>x</c> and gradient <c>g</c>, where generally the history
    ///         <c>m</c>can be short, often less than 10. These updates are used to implicitly do operations
    ///         requiring the Hk-vector product.
    ///     </para>
    ///     <para>
    ///         The framework implementation of this method is based on the original FORTRAN source code
    ///         by Jorge Nocedal (see references below). The original FORTRAN source code of L-BFGS (for
    ///         unconstrained problems) is available at http://www.netlib.org/opt/lbfgs_um.shar and had
    ///         been made available under the public domain.
    ///     </para>
    ///     <para>
    ///         References:
    ///         <list type="bullet">
    ///             <item>
    ///                 <description>
    ///                     <a href="http://www.netlib.org/opt/lbfgs_um.shar">
    ///                         Jorge Nocedal. Limited memory BFGS method for large scale optimization (Fortran source code).
    ///                         1990.
    ///                         Available in http://www.netlib.org/opt/lbfgs_um.shar
    ///                     </a>
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <description>
    ///                     Jorge Nocedal. Updating Quasi-Newton Matrices with Limited Storage.
    ///                     <i>Mathematics of Computation</i>,
    ///                     Vol. 35, No. 151, pp. 773--782, 1980.
    ///                 </description>
    ///             </item>
    ///             <item>
    ///                 <description>
    ///                     Dong C. Liu, Jorge Nocedal. On the limited memory BFGS method for large scale optimization.
    ///                 </description>
    ///             </item>
    ///         </list>
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <para>
    ///         The following example shows the basic usage of the L-BFGS solver
    ///         to find the minimum of a function specifying its function and
    ///         gradient.
    ///     </para>
    ///     <code>
    /// // Suppose we would like to find the minimum of the function
    /// // 
    /// //   f(x,y)  =  -exp{-(x-1)²} - exp{-(y-2)²/2}
    /// //
    /// 
    /// // First we need write down the function either as a named
    /// // method, an anonymous method or as a lambda function:
    /// 
    /// Func&lt;double[], double> f = (x) =>
    ///     -Math.Exp(-Math.Pow(x[0] - 1, 2)) - Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2));
    /// 
    /// // Now, we need to write its gradient, which is just the
    /// // vector of first partial derivatives del_f / del_x, as:
    /// //
    /// //   g(x,y)  =  { del f / del x, del f / del y }
    /// // 
    /// 
    /// Func&lt;double[], double[]> g = (x) => new double[] 
    /// {
    ///     // df/dx = {-2 e^(-    (x-1)^2) (x-1)}
    ///     2 * Math.Exp(-Math.Pow(x[0] - 1, 2)) * (x[0] - 1),
    /// 
    ///     // df/dy = {-  e^(-1/2 (y-2)^2) (y-2)}
    ///     Math.Exp(-0.5 * Math.Pow(x[1] - 2, 2)) * (x[1] - 2)
    /// };
    /// 
    /// // Finally, we can create the L-BFGS solver, passing the functions as arguments
    /// var lbfgs = new BroydenFletcherGoldfarbShanno(numberOfVariables: 2, function: f, gradient: g);
    /// 
    /// // And then minimize the function:
    /// bool success = lbfgs.Minimize();
    /// double minValue = lbfgs.Value;
    /// double[] solution = lbfgs.Solution;
    /// 
    /// // The resultant minimum value should be -2, and the solution
    /// // vector should be { 1.0, 2.0 }. The answer can be checked on
    /// // Wolfram Alpha by clicking the following the link:
    /// 
    /// // http://www.wolframalpha.com/input/?i=maximize+%28exp%28-%28x-1%29%C2%B2%29+%2B+exp%28-%28y-2%29%C2%B2%2F2%29%29
    /// 
    /// </code>
    /// </example>
    /// <seealso cref="ConjugateGradient" />
    /// <seealso cref="ResilientBackpropagation" />
    /// <seealso cref="BroydenFletcherGoldfarbShanno" />
    /// <seealso cref="TrustRegionNewtonMethod" />
    public partial class BoundedBroydenFletcherGoldfarbShanno : BaseGradientOptimizationMethod,
        IGradientOptimizationMethod, IOptimizationMethod<BoundedBroydenFletcherGoldfarbShannoStatus>,
        ISupportsCancellation
    {
        // those values need not be modified
        private const double stpmin = 1e-20;
        private const double stpmax = 1e20;

        private int corrections = 5;

        private double factr = 1e+5;

        // Line search parameters

        private double[] lowerBound;
        private double[] upperBound;

        private double[] work;
        /// <summary>
        ///     Implements the actual optimization algorithm. This
        ///     method should try to minimize the objective function.
        /// </summary>
        protected override bool Optimize()
        {
            if (Function == null)
                throw new InvalidOperationException("function");

            if (Gradient == null)
                throw new InvalidOperationException("gradient");

            NonlinearObjectiveFunction.CheckGradient(Gradient, Solution);
            var n = NumberOfVariables;
            var m = corrections;

            var task = "";
            var csave = "";
            var lsave = new bool[4];
            var iprint = 101;
            var nbd = new int[n];
            var iwa = new int[3 * n];
            var isave = new int[44];
            var f = 0.0d;
            var x = new double[n];
            var l = new double[n];
            var u = new double[n];
            var g = new double[n];
            var dsave = new double[29];

            var totalSize = 2 * m * n + 11 * m * m + 5 * n + 8 * m;

            if (work == null || work.Length < totalSize)
                work = new double[totalSize];

            var i = 0;

            {
                for (i = 0; i < UpperBounds.Length; i++)
                {
                    var hasUpper = !double.IsInfinity(UpperBounds[i]);
                    var hasLower = !double.IsInfinity(LowerBounds[i]);

                    if (hasUpper && hasLower)
                        nbd[i] = 2;
                    else if (hasUpper)
                        nbd[i] = 3;
                    else if (hasLower)
                        nbd[i] = 1;
                    else nbd[i] = 0; // unbounded

                    if (hasLower)
                        l[i] = LowerBounds[i];
                    if (hasUpper)
                        u[i] = UpperBounds[i];
                }
            }
            // We now define the starting point.
            {
                for (i = 0; i < n; i++)
                    x[i] = Solution[i];
            }

            double newF = 0;
            double[] newG = null;

            // We start the iteration by initializing task.
            task = "START";

            Iterations = 0;

        // 
        // c        ------- the beginning of the loop ----------
        // 
        L111:
            if (Token.IsCancellationRequested)
                return false;

            if (MaxIterations > 0 && Iterations >= MaxIterations)
            {
                exit(csave, lsave, isave, x, dsave, out newF, out newG);
                return true;
            }

            Iterations++;

            // 
            // c     This is the call to the L-BFGS-B code.
            // 
            setulb(n, m, x, 0, l, 0, u, 0, nbd, 0, ref f, g, 0,
                factr, GradientTolerance, work, 0, iwa, 0, ref task, iprint, ref csave,
                lsave, 0, isave, 0, dsave, 0);
            // 
            if (task.StartsWith("FG", StringComparison.OrdinalIgnoreCase))
            {
                newF = Function(x);
                newG = Gradient(x);
                Evaluations++;

                f = newF;

                for (var j = 0; j < newG.Length; j++)
                    g[j] = newG[j];
            }

            // c
            else if (task.StartsWith("NEW_X", StringComparison.OrdinalIgnoreCase))
            {
            }
            else
            {
                if (task == "ABNORMAL_TERMINATION_IN_LNSRCH")
                    Status = BoundedBroydenFletcherGoldfarbShannoStatus.LineSearchFailed;
                else if (task == "CONVERGENCE: REL_REDUCTION_OF_F_<=_FACTR*EPSMCH")
                    Status = BoundedBroydenFletcherGoldfarbShannoStatus.FunctionConvergence;
                else if (task == "CONVERGENCE: NORM_OF_PROJECTED_GRADIENT_<=_PGTOL")
                    Status = BoundedBroydenFletcherGoldfarbShannoStatus.GradientConvergence;
                else throw OperationException(task, task);

                exit(csave, lsave, isave, x, dsave, out newF, out newG);
                return true;
            }
            if (Progress != null)
                Progress(this, new OptimizationProgressEventArgs(Iterations, 0, newG, 0, null, 0, f, 0, false)
                {
                    Tag = new BoundedBroydenFletcherGoldfarbShannoInnerStatus(
                        isave, dsave, lsave, csave, work)
                });

            goto L111;
        }

        private void exit(string csave, bool[] lsave, int[] isave, double[] x, double[] dsave, out double newF,
            out double[] newG)
        {
            for (var j = 0; j < Solution.Length; j++)
                Solution[j] = x[j];

            newF = Function(x);
            newG = Gradient(x);
            Evaluations++;

            if (Progress != null)
                Progress(this, new OptimizationProgressEventArgs(Iterations, 0, newG, 0, null, 0, newF, 0, true)
                {
                    Tag = new BoundedBroydenFletcherGoldfarbShannoInnerStatus(
                        isave, dsave, lsave, csave, work)
                });
        }
        #region Properties

        /// <summary>
        ///     Occurs when progress is made during the optimization.
        /// </summary>
        public event EventHandler<OptimizationProgressEventArgs> Progress;
        /// <summary>
        ///     Gets the number of iterations performed in the last
        ///     call to <see cref="IOptimizationMethod{TInput, TOutput}.Minimize()" />
        ///     or <see cref="IOptimizationMethod{TInput, TOutput}.Maximize()" />.
        /// </summary>
        /// <value>
        ///     The number of iterations performed
        ///     in the previous optimization.
        /// </value>
        public int Iterations { get; private set; }

        /// <summary>
        ///     Gets or sets the maximum number of iterations
        ///     to be performed during optimization. Default
        ///     is 0 (iterate until convergence).
        /// </summary>
        public int MaxIterations { get; set; }

        /// <summary>
        ///     Gets the number of function evaluations performed
        ///     in the last call to <see cref="IOptimizationMethod{TInput, TOutput}.Minimize()" />
        ///     or <see cref="IOptimizationMethod{TInput, TOutput}.Maximize()" />.
        /// </summary>
        /// <value>
        ///     The number of evaluations performed
        ///     in the previous optimization.
        /// </value>
        public int Evaluations { get; private set; }

        /// <summary>
        ///     Gets or sets the number of corrections used in the L-BFGS
        ///     update. Recommended values are between 3 and 7. Default is 5.
        /// </summary>
        public int Corrections
        {
            get => corrections;
            set
            {
                if (value <= 0)
                    throw ArgumentException("value",
                        "Number of corrections should be higher than zero.", "ERROR: M .LE. 0");

                corrections = value;
            }
        }

        /// <summary>
        ///     Gets or sets the upper bounds of the interval
        ///     in which the solution must be found.
        /// </summary>
        public double[] UpperBounds
        {
            get => upperBound;
            set
            {
                if (value.Length != upperBound.Length)
                    throw new DimensionMismatchException("value",
                        $"The bounds vector should have the same length as the number of variables to be optimized ({NumberOfVariables}).");
                upperBound = value;
            }
        }

        /// <summary>
        ///     Gets or sets the lower bounds of the interval
        ///     in which the solution must be found.
        /// </summary>
        public double[] LowerBounds
        {
            get => lowerBound;
            set
            {
                if (value.Length != lowerBound.Length)
                    throw new DimensionMismatchException("value",
                        $"The bounds vector should have the same length as the number of variables to be optimized ({NumberOfVariables}).");
                lowerBound = value;
            }
        }

        /// <summary>
        ///     Gets or sets the accuracy with which the solution
        ///     is to be found. Default value is 1e5. Smaller values
        ///     up until zero result in higher accuracy.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The iteration will stop when
        ///     </para>
        ///     <code>
        ///          (f^k - f^{k+1})/max{|f^k|,|f^{k+1}|,1} &lt;= factr*epsmch </code>
        ///     <para>
        ///         where epsmch is the machine precision, which is automatically
        ///         generated by the code. Typical values for this parameter are:
        ///         1e12 for low accuracy; 1e7 for moderate accuracy; 1e1 for extremely
        ///         high accuracy.
        ///     </para>
        /// </remarks>
        public double FunctionTolerance
        {
            get => factr;
            set
            {
                if (value < 0)
                    throw ArgumentException("value",
                        "Tolerance must be greater than or equal to zero.", "ERROR: FACTR .LT. 0");

                factr = value;
            }
        }

        /// <summary>
        ///     Gets or sets a tolerance value when detecting convergence
        ///     of the gradient vector steps. Default is 0.
        /// </summary>
        /// <remarks>
        ///     On entry pgtol >= 0 is specified by the user.  The iteration
        ///     will stop when
        ///     <code>
        ///   max{|proj g_i || i = 1, ..., n} &lt;= pgtol
        /// </code>
        ///     <para>
        ///         where pg_i is the ith component of the projected gradient.
        ///     </para>
        /// </remarks>
        public double GradientTolerance { get; set; } = 0.0;

        /// <summary>
        ///     Get the exit code returned in the last call to the
        ///     <see cref="IOptimizationMethod{TInput, TOutput}.Maximize()" /> or
        ///     <see cref="IOptimizationMethod{TInput, TOutput}.Minimize()" /> methods.
        /// </summary>
        public BoundedBroydenFletcherGoldfarbShannoStatus Status { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates a new instance of the L-BFGS optimization algorithm.
        /// </summary>
        /// <param name="numberOfVariables">The number of free parameters in the optimization problem.</param>
        public BoundedBroydenFletcherGoldfarbShanno(int numberOfVariables)
            : base(numberOfVariables)
        {
            upperBound = new double[numberOfVariables];
            lowerBound = new double[numberOfVariables];

            for (var i = 0; i < upperBound.Length; i++)
                lowerBound[i] = double.NegativeInfinity;

            for (var i = 0; i < upperBound.Length; i++)
                upperBound[i] = double.PositiveInfinity;
        }

        /// <summary>
        ///     Creates a new instance of the L-BFGS optimization algorithm.
        /// </summary>
        /// <param name="numberOfVariables">The number of free parameters in the function to be optimized.</param>
        /// <param name="function">The function to be optimized.</param>
        /// <param name="gradient">The gradient of the function.</param>
        public BoundedBroydenFletcherGoldfarbShanno(int numberOfVariables,
            Func<double[], double> function, Func<double[], double[]> gradient)
            : this(numberOfVariables)
        {
            if (function == null)
                throw new ArgumentNullException("function");

            if (gradient == null)
                throw new ArgumentNullException("gradient");

            Function = function;
            Gradient = gradient;
        }

        #endregion
    }
}