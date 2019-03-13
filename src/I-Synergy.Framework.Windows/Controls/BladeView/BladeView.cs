﻿using ISynergy.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ISynergy.Controls
{
    /// <summary>
    /// A container that hosts <see cref="BladeItem"/> controls in a horizontal scrolling list
    /// Based on the Azure portal UI
    /// </summary>
    public partial class BladeView : ItemsControl
    {
        private ScrollViewer _scrollViewer;

        private readonly Dictionary<BladeItem, Size> _cachedBladeItemSizes = new Dictionary<BladeItem, Size>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BladeView"/> class.
        /// </summary>
        public BladeView()
        {
            DefaultStyleKey = typeof(BladeView);

            Items.VectorChanged += ItemsVectorChanged;

            Loaded += (s, e) => AdjustBladeItemSize();
            SizeChanged += (s, e) => AdjustBladeItemSize();
        }

        /// <inheritdoc/>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CycleBlades();
            AdjustBladeItemSize();
        }

        /// <inheritdoc/>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new BladeItem();
        }

        /// <inheritdoc/>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is BladeItem;
        }

        /// <inheritdoc/>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is BladeItem blade)
            {
                blade.VisibilityChanged += BladeOnVisibilityChanged;
            }

            base.PrepareContainerForItemOverride(element, item);
            CycleBlades();
        }

        /// <inheritdoc/>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            if (element is BladeItem blade)
            {
                blade.VisibilityChanged -= BladeOnVisibilityChanged;
            }

            base.ClearContainerForItemOverride(element, item);
        }

        private void CycleBlades()
        {
            ActiveBlades = new ObservableCollection<BladeItem>();
            foreach (var item in Items)
            {
                var blade = GetBladeItem(item);
                if (blade != null)
                {
                    if (blade.IsOpen)
                    {
                        ActiveBlades.Add(blade);
                    }
                }
            }

            // For now we skip this feature when blade mode is set to fullscreen
            if (AutoCollapseCountThreshold > 0 && BladeMode != BladeMode.Fullscreen && ActiveBlades.Any())
            {
                var openBlades = ActiveBlades.Where(item => item.TitleBarVisibility == Visibility.Visible).ToList();
                if (openBlades.Count > AutoCollapseCountThreshold)
                {
                    for (var i = 0; i < openBlades.Count - 1; i++)
                    {
                        openBlades[i].IsExpanded = false;
                    }
                }
            }
        }

        private BladeItem GetBladeItem(object item)
        {
            if (!(item is BladeItem blade))
            {
                blade = (BladeItem)ContainerFromItem(item);
            }

            return blade;
        }

        private async void BladeOnVisibilityChanged(object sender, Visibility visibility)
        {
            var blade = sender as BladeItem;

            if (visibility == Visibility.Visible)
            {
                if (Items == null)
                {
                    return;
                }

                var item = ItemFromContainer(blade);
                Items.Remove(item);
                Items.Add(item);
                BladeOpened?.Invoke(this, blade);
                ActiveBlades.Add(blade);
                UpdateLayout();

                // Need to do this because of touch. See more information here: https://github.com/windows-toolkit/WindowsCommunityToolkit/issues/760#issuecomment-276466464
                await Dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                {
                    GetScrollViewer()?.ChangeView(_scrollViewer.ScrollableWidth, null, null);
                });

                return;
            }

            BladeClosed?.Invoke(this, blade);
            ActiveBlades.Remove(blade);

            var lastBlade = ActiveBlades.LastOrDefault();
            if (lastBlade != null && lastBlade.TitleBarVisibility == Visibility.Visible)
            {
                lastBlade.IsExpanded = true;
            }
        }

        private ScrollViewer GetScrollViewer()
        {
            return _scrollViewer ?? (_scrollViewer = this.FindDescendant<ScrollViewer>());
        }

        private void AdjustBladeItemSize()
        {
            // Adjust blade items to be full screen
            if (BladeMode == BladeMode.Fullscreen && GetScrollViewer() != null)
            {
                foreach (var item in Items)
                {
                    var blade = GetBladeItem(item);
                    blade.Width = _scrollViewer.ActualWidth;
                    blade.Height = _scrollViewer.ActualHeight;
                }
            }
        }

        private void ItemsVectorChanged(IObservableVector<object> sender, IVectorChangedEventArgs e)
        {
            if (BladeMode == BladeMode.Fullscreen)
            {
                var bladeItem = GetBladeItem(sender[(int)e.Index]);
                if (bladeItem != null)
                {
                    if (!_cachedBladeItemSizes.ContainsKey(bladeItem))
                    {
                        // Execute change of blade item size when a blade item is added in Fullscreen mode
                        _cachedBladeItemSizes.Add(bladeItem, new Size(bladeItem.Width, bladeItem.Height));
                        AdjustBladeItemSize();
                    }
                }
            }
            else if (e.CollectionChange == CollectionChange.ItemInserted)
            {
                UpdateLayout();
                GetScrollViewer()?.ChangeView(_scrollViewer.ScrollableWidth, null, null);
            }
        }
    }
}
