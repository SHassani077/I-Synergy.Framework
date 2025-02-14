﻿using ISynergy.Framework.Core.Validation;
using ISynergy.Framework.UI.Extensions;
using System;

#if (WINDOWS_UWP || HAS_UNO)
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Data;
#else
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Data;
#endif

namespace ISynergy.Framework.UI.Converters
{
    /// <summary>
    /// Class SolidColorBrushToHexStringConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class SolidColorBrushToHexStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Argument.IsNotNull(nameof(value), value);
            return ((SolidColorBrush)value).Color.ToString();
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Class IntegerToBrushConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class IntegerToBrushConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is int color && color != 0)
            {
                return new SolidColorBrush(color.ConvertInteger2Color());
            }

            return null;
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Class IntegerToColorConverter.
    /// Implements the <see cref="IValueConverter" />
    /// </summary>
    /// <seealso cref="IValueConverter" />
    public class IntegerToColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Argument.IsNotNull(nameof(value), value);

            if (value is int color)
            {
                return color.ConvertInteger2Color();
            }

            return null;
        }

        /// <summary>
        /// Converts the back.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>System.Object.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Argument.IsNotNull(nameof(value), value);

            if(value is Windows.UI.Color color)
            {
                return color.ConvertColor2Integer();
            }

            return null;
        }
    }
}
