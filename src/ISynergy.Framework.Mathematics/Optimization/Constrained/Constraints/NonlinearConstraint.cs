﻿using ISynergy.Framework.Mathematics;
using ISynergy.Framework.Mathematics.Exceptions;
using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ISynergy.Framework.Mathematics.Optimization.Constrained.Constraints
{
    /// <summary>
    ///     Constraint with only linear terms.
    /// </summary>
    public class NonlinearConstraint : IConstraint, IFormattable
    {
        private const double DEFAULT_TOL = 1e-8;

        private Func<double[], double> function;

        private Func<double[], double[]> gradient;

#if !NET35
        /// <summary>
        ///     Constructs a new nonlinear constraint.
        /// </summary>
        /// <param name="objective">The objective function to which this constraint refers.</param>
        /// <param name="function">A lambda expression defining the left hand side of the constraint equation.</param>
        /// <param name="gradient">
        ///     A lambda expression defining the gradient of the
        ///     <paramref name="function">
        ///         left hand side of the constraint equation
        ///     </paramref>
        ///     .
        /// </param>
        /// <param name="shouldBe">
        ///     How the left hand side of the constraint
        ///     should be compared to the given <paramref name="value" />.
        /// </param>
        /// <param name="value">The right hand side of the constraint equation.</param>
        /// <param name="withinTolerance">
        ///     The tolerance for violations of the constraint. Equality
        ///     constraints should set this to a small positive value. Default is 1e-8.
        /// </param>
        public NonlinearConstraint(IObjectiveFunction objective,
            Expression<Func<double>> function, ConstraintType shouldBe, double value,
            Expression<Func<double[]>> gradient = null, double withinTolerance = DEFAULT_TOL)
        {
            NumberOfVariables = objective.NumberOfVariables;
            ShouldBe = shouldBe;

            // Generate lambda functions
            var func = ExpressionParser.Replace(function, objective.Variables);
            this.function = func.Compile();
            Value = value;
            Tolerance = withinTolerance;

            if (gradient != null)
            {
                var grad = ExpressionParser.Replace(gradient, objective.Variables);
                this.gradient = grad.Compile();

                var n = NumberOfVariables;
                var probe = new double[n];
                var g = Gradient(probe);

                if (g.Length != n)
                    throw new DimensionMismatchException("gradient",
                        "The length of the gradient vector must match the number of variables in the objective function.");
            }
        }
#endif

        /// <summary>
        ///     Constructs a new nonlinear constraint.
        /// </summary>
        /// <param name="objective">The objective function to which this constraint refers.</param>
        /// <param name="constraint">
        ///     A boolean lambda expression expressing the constraint. Please
        ///     see examples for details.
        /// </param>
        /// <param name="gradient">
        ///     A lambda expression defining the gradient of the
        ///     <paramref name="constraint">
        ///         left hand side of the constraint equation
        ///     </paramref>
        ///     .
        /// </param>
        public NonlinearConstraint(IObjectiveFunction objective,
            Expression<Func<double[], bool>> constraint,
            Func<double[], double[]> gradient = null)
        {
            Func<double[], double> function;
            ConstraintType shouldBe;
            double value;

            parse(constraint, out function, out shouldBe, out value);

            Create(objective.NumberOfVariables, function, shouldBe, value, gradient, DEFAULT_TOL);
        }

        /// <summary>
        ///     Constructs a new nonlinear constraint.
        /// </summary>
        /// <param name="numberOfVariables">The number of variables in the constraint.</param>
        /// <param name="constraint">
        ///     A boolean lambda expression expressing the constraint. Please
        ///     see examples for details.
        /// </param>
        /// <param name="gradient">
        ///     A lambda expression defining the gradient of the
        ///     <paramref name="constraint">
        ///         left hand side of the constraint equation
        ///     </paramref>
        ///     .
        /// </param>
        public NonlinearConstraint(int numberOfVariables,
            Expression<Func<double[], bool>> constraint,
            Func<double[], double[]> gradient = null)
        {
            Func<double[], double> function;
            ConstraintType shouldBe;
            double value;

            parse(constraint, out function, out shouldBe, out value);

            Create(numberOfVariables, function, shouldBe, value, gradient, DEFAULT_TOL);
        }

        /// <summary>
        ///     Constructs a new nonlinear constraint.
        /// </summary>
        /// <param name="objective">The objective function to which this constraint refers.</param>
        /// <param name="function">A lambda expression defining the left hand side of the constraint equation.</param>
        /// <param name="gradient">
        ///     A lambda expression defining the gradient of the
        ///     <paramref name="function">
        ///         left hand side of the constraint equation
        ///     </paramref>
        ///     .
        /// </param>
        /// <param name="shouldBe">
        ///     How the left hand side of the constraint should be compared to the given
        ///     <paramref name="value" />.
        /// </param>
        /// <param name="value">The right hand side of the constraint equation. Default is 0.</param>
        /// <param name="withinTolerance">
        ///     The tolerance for violations of the constraint. Equality
        ///     constraints should set this to a small positive value. Default is 1e-8.
        /// </param>
        public NonlinearConstraint(IObjectiveFunction objective,
            Func<double[], double> function,
            ConstraintType shouldBe = ConstraintType.GreaterThanOrEqualTo,
            double value = 0,
            Func<double[], double[]> gradient = null,
            double withinTolerance = DEFAULT_TOL)
        {
            Create(objective.NumberOfVariables, function, shouldBe, value, gradient, withinTolerance);
        }
        /// <summary>
        ///     Constructs a new nonlinear constraint.
        /// </summary>
        /// <param name="numberOfVariables">The number of variables in the constraint.</param>
        /// <param name="function">A lambda expression defining the left hand side of the constraint equation.</param>
        /// <param name="gradient">
        ///     A lambda expression defining the gradient of the
        ///     <paramref name="function">
        ///         left hand side of the constraint equation
        ///     </paramref>
        ///     .
        /// </param>
        /// <param name="shouldBe">
        ///     How the left hand side of the constraint should be compared to the given
        ///     <paramref name="value" />.
        /// </param>
        /// <param name="value">The right hand side of the constraint equation. Default is 0.</param>
        /// <param name="withinTolerance">
        ///     The tolerance for violations of the constraint. Equality
        ///     constraints should set this to a small positive value. Default is 1e-8.
        /// </param>
        public NonlinearConstraint(int numberOfVariables,
            Func<double[], double> function,
            ConstraintType shouldBe = ConstraintType.GreaterThanOrEqualTo,
            double value = 0,
            Func<double[], double[]> gradient = null,
            double withinTolerance = DEFAULT_TOL)
        {
            Create(numberOfVariables, function, shouldBe, value, gradient, withinTolerance);
        }

        /// <summary>
        ///     Creates an empty nonlinear constraint.
        /// </summary>
        protected NonlinearConstraint()
        {
        }

        /// <summary>
        ///     Gets the number of variables in the constraint.
        /// </summary>
        public int NumberOfVariables { get; private set; }

        /// <summary>
        ///     Gets the type of the constraint.
        /// </summary>
        public ConstraintType ShouldBe { get; private set; }

        /// <summary>
        ///     Gets the value in the right hand side of
        ///     the constraint equation. Default is 0.
        /// </summary>
        public double Value { get; private set; }

        /// <summary>
        ///     Gets the violation tolerance for the constraint. Equality
        ///     constraints should set this to a small positive value.
        ///     Default is 1e-8.
        /// </summary>
        public double Tolerance { get; set; }

        /// <summary>
        ///     Calculates the left hand side of the constraint
        ///     equation given a vector x.
        /// </summary>
        /// <param name="x">The vector.</param>
        /// <returns>
        ///     The left hand side of the constraint equation as evaluated at x.
        /// </returns>
        public double Function(double[] x)
        {
            return function(x);
        }

        /// <summary>
        ///     Calculates the gradient of the constraint
        ///     equation given a vector x
        /// </summary>
        /// <param name="x">The vector.</param>
        /// <returns>The gradient of the constraint as evaluated at x.</returns>
        public double[] Gradient(double[] x)
        {
            return gradient(x);
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            var v = Value.ToString(format, formatProvider);
            var t = Tolerance.ToString(format, formatProvider);

            switch (ShouldBe)
            {
                case ConstraintType.EqualTo:
                    return string.Format(formatProvider, "g(x) == {0} (± {1})", v, t);

                case ConstraintType.GreaterThanOrEqualTo:
                    return string.Format(formatProvider, "g(x) >= {0} (± {1})", v, t);

                case ConstraintType.LesserThanOrEqualTo:
                    return string.Format("g(x) <= {0} (± {1})", v, t);
            }

            return "g(x)";
        }

        /// <summary>
        ///     Creates a nonlinear constraint.
        /// </summary>
        protected void Create(int numberOfVariables,
            Func<double[], double> function, ConstraintType shouldBe, double value,
            Func<double[], double[]> gradient, double tolerance)
        {
            if (gradient != null)
            {
                var probe = new double[numberOfVariables];
                var g = gradient(probe);

                if (g.Length != numberOfVariables)
                    throw new DimensionMismatchException("gradient",
                        "The length of the gradient vector must match the number of variables in the objective function.");
            }

            NumberOfVariables = numberOfVariables;
            ShouldBe = shouldBe;
            Value = value;
            Tolerance = tolerance;

            this.function = function;
            this.gradient = gradient;
        }
        private static void parse(Expression<Func<double[], bool>> constraint,
            out Func<double[], double> function, out ConstraintType shouldBe, out double value)
        {
            var expression = constraint.Body as BinaryExpression;

            var comparisonType = expression.NodeType;

            switch (comparisonType)
            {
                case ExpressionType.LessThanOrEqual:
                    shouldBe = ConstraintType.LesserThanOrEqualTo;
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    shouldBe = ConstraintType.GreaterThanOrEqualTo;
                    break;

                case ExpressionType.Equal:
                    shouldBe = ConstraintType.EqualTo;
                    break;

                default:
                    throw new NotSupportedException(comparisonType + " is not supported.");
            }

            var left = expression.Left;

            // Try to determine the right side of the constraint expression. Normally
            // this should be a constant, a variable (field), or a property. Other cases
            // are not supported for the moment.

            value = double.NaN;
            var rightSideParsed = false;

            var right = expression.Right as ConstantExpression;
            if (right != null)
            {
                // Right side is a constant
                value = (double)right.Value;
                rightSideParsed = true;
            }
            else
            {
                // Right side is not a constant, try to determine what it is
                var variableRight = expression.Right as MemberExpression;
                if (variableRight != null)
                {
                    // Right side is a member variable: a field, a property or something else
                    var field = variableRight.Member as FieldInfo;
                    if (field != null)
                    {
                        // Right side is a field
                        var constant = variableRight.Expression as ConstantExpression;
                        value = (double)field.GetValue(constant.Value);
                        rightSideParsed = true;
                    }
                    else
                    {
                        // Right side is not a field, try to determine what it is
                        var prop = variableRight.Member as PropertyInfo;
                        if (prop != null)
                        {
                            // Right side is a property
                            var constant = variableRight.Expression as ConstantExpression;
                            value = (double)prop.GetValue(constant.Value);
                            rightSideParsed = true;
                        }
                    }
                }
            }

            if (!rightSideParsed)
                throw new ArgumentException(
                    "The right side of the expression could not be parsed. Please post the code you are trying to execute as a new issue in ISynergy.Framework.Mathematics.NET's issue tracker.");

            var functionExpression = Expression.Lambda(left, constraint.Parameters.ToArray());

            function = functionExpression.Compile() as Func<double[], double>;
        }
        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="string" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ToString("G", CultureInfo.CurrentCulture);
        }
    }
}