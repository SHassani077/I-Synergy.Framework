﻿using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.EntityFramework.Events
{
    /// <summary>
    /// A container for entities that have been inserted.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityInsertedEvent<T> where T : EntityBase
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="entity">Entity</param>
        public EntityInsertedEvent(T entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// Entity
        /// </summary>
        /// <value>The entity.</value>
        public T Entity { get; }
    }
}
