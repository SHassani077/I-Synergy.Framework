﻿using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Core.Base;

namespace ISynergy.Framework.UI.Services
{
    /// <summary>
    /// Class BusyService.
    /// Implements the <see cref="ObservableClass" />
    /// Implements the <see cref="IBusyService" />
    /// </summary>
    /// <seealso cref="ObservableClass" />
    /// <seealso cref="IBusyService" />
    public class BusyService : ObservableClass, IBusyService
    {
        /// <summary>
        /// The language service
        /// </summary>
        protected readonly ILanguageService LanguageService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BusyService"/> class.
        /// </summary>
        /// <param name="language">The language.</param>
        public BusyService(ILanguageService language)
        {
            LanguageService = language;
        }

        /// <summary>
        /// Gets or sets the IsBusy property value.
        /// </summary>
        /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
        public bool IsBusy
        {
            get => GetValue<bool>();
            set
            {
                SetValue(value);
                IsEnabled = !value;
            }
        }

        /// <summary>
        /// Gets or sets the IsEnabled property value.
        /// </summary>
        /// <value><c>true</c> if this instance is enabled; otherwise, <c>false</c>.</value>
        public bool IsEnabled
        {
            get => GetValue<bool>();
            set => SetValue(value);
        }

        /// <summary>
        /// Gets or sets the BusyMessage property value.
        /// </summary>
        /// <value>The busy message.</value>
        public string BusyMessage
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        /// <summary>
        /// Starts the busy asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public void StartBusy(string message = null)
        {
            if (message != null)
            {
                BusyMessage = message;
            }
            else
            {
                BusyMessage = LanguageService.GetString("PleaseWait");
            }

            IsBusy = true;
        }

        /// <summary>
        /// Starts the busy asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public void StartBusy() =>
            StartBusy(LanguageService.GetString("PleaseWait"));

        /// <summary>
        /// Ends the busy asynchronous.
        /// </summary>
        /// <returns>Task.</returns>
        public void EndBusy() => IsBusy = false;
    }
}
