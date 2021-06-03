﻿using System;
using ISynergy.Framework.Mvvm.Enumerations;

namespace ISynergy.Framework.UI.Abstractions.Services
{
    /// <summary>
    /// Interface IThemeSelectorService
    /// </summary>
    public interface IThemeSelectorService
    {
        /// <summary>
        /// Gets a value indicating whether this instance is light theme enabled.
        /// </summary>
        /// <value><c>true</c> if this instance is light theme enabled; otherwise, <c>false</c>.</value>
        bool IsLightThemeEnabled { get; }
        /// <summary>
        /// Gets or sets the theme.
        /// </summary>
        /// <value>The theme.</value>
        object Theme { get; set; }

        /// <summary>
        /// Occurs when [on theme changed].
        /// </summary>
        event EventHandler<object> OnThemeChanged;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        void Initialize();
        /// <summary>
        /// Sets the requested theme.
        /// </summary>
        void SetThemeColor(ThemeColors color);
        /// <summary>
        /// Sets the theme.
        /// </summary>
        /// <param name="theme">The theme.</param>
        void SetTheme(object theme);
        /// <summary>
        /// Switches the theme.
        /// </summary>
        void SwitchTheme();

        /// <summary>
        /// AccentColor property.
        /// </summary>
        string AccentColor { get; }

        /// <summary>
        /// Light theme AccentColor property.
        /// </summary>
        string LightAccentColor { get; }

        /// <summary>
        /// Dark theme AccentColor property.
        /// </summary>
        string DarkAccentColor { get; }
    }
}
