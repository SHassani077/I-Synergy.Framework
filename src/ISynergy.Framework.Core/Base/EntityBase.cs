﻿using ISynergy.Framework.Core.Abstractions.Base;
using System;
using System.Reflection;

namespace ISynergy.Framework.Core.Base
{
    /// <summary>
    /// BaseEntity model which fully supports serialization, property changed notifications,
    /// backwards compatibility and error checking.
    /// </summary>
    public class EntityBase : ClassBase, IEntityBase
    {
        /// <summary>
        /// Gets or sets the memo.
        /// </summary>
        /// <value>The memo.</value>
        public string Memo { get; set; }
        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        /// <value>The created date.</value>
        public DateTimeOffset CreatedDate { get; set; }
        /// <summary>
        /// Gets or sets the changed date.
        /// </summary>
        /// <value>The changed date.</value>
        public DateTimeOffset? ChangedDate { get; set; }
        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>The created by.</value>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Gets or sets the changed by.
        /// </summary>
        /// <value>The changed by.</value>
        public string ChangedBy { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityBase"/> class.
        /// </summary>
        public EntityBase()
        {
            CreatedBy = string.Empty;
        }
    }

    /// <summary>
    /// Class EntityBaseExtensions.
    /// </summary>
    public static class EntityBaseExtensions
    {
        /// <summary>
        /// Determines whether the specified property name has property.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><c>true</c> if the specified property name has property; otherwise, <c>false</c>.</returns>
        public static bool HasProperty(this object obj, string propertyName)
        {
            try
            {
                return obj.GetType().GetRuntimeProperty(propertyName) != null;
            }
            catch (AmbiguousMatchException)
            {
                // ambiguous means there is more than one result,
                // which means: a method with that name does exist
                return true;
            }
        }
    }
}
