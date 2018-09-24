﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ISynergy.Converters
{
    public class GuidToInversedVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is Guid && (Guid)value != Guid.Empty)
            {
                return Visibility.Collapsed;
            }
            else
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GuidToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string culture)
        {
            if (value is Guid && (Guid)value != Guid.Empty)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string culture)
        {
            throw new NotImplementedException();
        }
    }
}
