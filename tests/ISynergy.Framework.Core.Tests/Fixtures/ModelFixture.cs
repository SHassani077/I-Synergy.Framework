﻿using System;
using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.Core.Fixtures
{
    /// <summary>
    /// Class ModelFixture.
    /// Implements the <see cref="Base.ModelBase" />
    /// Implements the <see cref="System.IDisposable" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Base.ModelBase" />
    /// <seealso cref="System.IDisposable" />
    public class ModelFixture<T> : ModelBase, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelFixture{T}"/> class.
        /// </summary>
        public ModelFixture()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModelFixture{T}"/> class.
        /// </summary>
        /// <param name="initialValue">The initial value.</param>
        public ModelFixture(T initialValue)
            : this()
        {
            Value = initialValue;
        }

        /// <summary>
        /// Gets or sets the Value property value.
        /// </summary>
        /// <value>The value.</value>
        public T Value
        {
            get { return GetValue<T>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            if (Value is null)
            {
                return string.Empty;
            }
            else
            {
                return Value.ToString();
            }
        }
    }
}
