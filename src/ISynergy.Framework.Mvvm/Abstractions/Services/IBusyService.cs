﻿using System.Threading.Tasks;

namespace ISynergy.Framework.Mvvm.Abstractions.Services
{
    /// <summary>
    /// Interface IBusyService
    /// </summary>
    public interface IBusyService
    {
        /// <summary>
        /// Starts the busy.
        /// </summary>
        void StartBusy();
        /// <summary>
        /// Starts the busy.
        /// </summary>
        /// <param name="message">The message.</param>
        void StartBusy(string message);
        /// <summary>
        /// Ends the busy.
        /// </summary>
        void EndBusy();
        /// <summary>
        /// Gets or sets the busy message.
        /// </summary>
        /// <value>The busy message.</value>
        string BusyMessage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
        bool IsBusy { get; set; }
    }
}
