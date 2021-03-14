﻿using ISynergy.Framework.Core.Data;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Mvvm.Abstractions;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.Mvvm.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace ISynergy.Framework.UI
{
    public sealed partial class SelectionView : IView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionView"/> class.
        /// </summary>
        public SelectionView()
        {
            InitializeComponent();
            DataContextChanged += SelectionView_DataContextChanged;
            DataSummary.SelectionChanged += DataSummary_SelectionChanged;
        }

        private void DataSummary_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is ISelectionViewModel viewModel)
            {
                viewModel.SelectedItem = new List<object>();

                if (viewModel.SelectionMode == SelectionModes.Single)
                {
                    viewModel.SelectedItem.Add(DataSummary.SelectedItem);
                }
                else
                {
                    foreach (var item in DataSummary.SelectedItems)
                    {
                        viewModel.SelectedItem.Add(item);
                    }
                }
            }
        }

        private void SelectionView_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (DataContext is ISelectionViewModel viewModel)
            {
                if (viewModel.SelectionMode == SelectionModes.Single && viewModel.SelectedItem is not null && viewModel.SelectedItem.Count == 1)
                {
                    DataSummary.SelectedItem = viewModel.SelectedItem.Single();
                }
                else
                {
                    foreach (var item in viewModel.SelectedItem)
                    {
                        var index = DataSummary.Items.IndexOf(item);
                    }

                    //foreach (ItemIndexRange item in viewModel.SelectedItems.EnsureNotNull())
                    //{
                    //    DataSummary.SelectRange(item);
                    //};
                }
            }
        }
    }
}
