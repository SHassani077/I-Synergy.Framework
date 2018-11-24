﻿using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ISynergy.Behaviours.Base
{
    /// <summary>
    /// Base class for behaviors that solves 2 problems:
    ///   1. Prevent duplicate initialization that can happen (prevent multiple OnAttached calls);
    ///   2. Whenever <see cref="Initialize"/> initially fails, this method will subscribe to <see cref="FrameworkElement.SizeChanged"/> to allow lazy initialization.
    /// </summary>
    /// <typeparam name="T">The type of the associated object.</typeparam>
    /// <seealso cref="Microsoft.Xaml.Interactivity.Behavior{T}" />
    /// <remarks>
    /// For more info, see https://github.com/windows-toolkit/WindowsCommunityToolkit/issues/1008.
    /// </remarks>
    public abstract class BehaviorBase<T> : Behavior<T>
        where T : UIElement
    {
        private bool _isAttaching;
        private bool _isAttached;

        /// <summary>
        /// Gets a value indicating whether this behavior is attached.
        /// </summary>
        /// <value>
        /// <c>true</c> if this behavior is attached; otherwise, <c>false</c>.
        /// </value>
        protected bool IsAttached
        {
            get { return _isAttached; }
        }

        /// <summary>
        /// Called after the behavior is attached to the <see cref="P:Microsoft.Xaml.Interactivity.Behavior.AssociatedObject" />.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the <see cref="P:Microsoft.Xaml.Interactivity.Behavior.AssociatedObject" />
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();

            HandleAttach();

            if (AssociatedObject is FrameworkElement frameworkElement)
            {
                frameworkElement.Loaded += OnAssociatedObjectLoaded;
                frameworkElement.Unloaded += OnAssociatedObjectUnloaded;
                frameworkElement.SizeChanged += OnAssociatedObjectSizeChanged;
            }
        }

        /// <summary>
        /// Called when the behavior is being detached from its <see cref="P:Microsoft.Xaml.Interactivity.Behavior.AssociatedObject" />.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the <see cref="P:Microsoft.Xaml.Interactivity.Behavior.AssociatedObject" />
        /// </remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (AssociatedObject is FrameworkElement frameworkElement)
            {
                frameworkElement.Loaded -= OnAssociatedObjectLoaded;
                frameworkElement.Unloaded -= OnAssociatedObjectUnloaded;
                frameworkElement.SizeChanged -= OnAssociatedObjectSizeChanged;
            }

            HandleDetach();
        }

        /// <summary>
        /// Called when the associated object has been loaded.
        /// </summary>
        protected virtual void OnAssociatedObjectLoaded()
        {
        }

        /// <summary>
        /// Called when the associated object has been unloaded.
        /// </summary>
        protected virtual void OnAssociatedObjectUnloaded()
        {
        }

        /// <summary>
        /// Initializes the behavior to the associated object.
        /// </summary>
        /// <returns><c>true</c> if the initialization succeeded; otherwise <c>false</c>.</returns>
        protected virtual bool Initialize()
        {
            return true;
        }

        /// <summary>
        /// Uninitializes the behavior from the associated object.
        /// </summary>
        /// <returns><c>true</c> if uninitialization succeeded; otherwise <c>false</c>.</returns>
        protected virtual bool Uninitialize()
        {
            return true;
        }

        private void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e)
        {
            if (!_isAttached)
            {
                HandleAttach();
            }

            OnAssociatedObjectLoaded();
        }

        private void OnAssociatedObjectUnloaded(object sender, RoutedEventArgs e)
        {
            OnAssociatedObjectUnloaded();

            // Note: don't detach here, we'll let the behavior implementation take care of that
        }

        private void OnAssociatedObjectSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!_isAttached)
            {
                HandleAttach();
            }
        }

        private void HandleAttach()
        {
            if (_isAttaching || _isAttached)
            {
                return;
            }

            _isAttaching = true;

            var attached = Initialize();
            if (attached)
            {
                _isAttached = true;
            }

            _isAttaching = false;
        }

        private void HandleDetach()
        {
            if (!_isAttached)
            {
                return;
            }

            var detached = Uninitialize();
            if (detached)
            {
                _isAttached = false;
            }
        }
    }
}
