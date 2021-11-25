﻿using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Monitoring.Client.Abstractions.Services;
using ISynergy.Framework.Monitoring.Common.Options;
using ISynergy.Framework.Monitoring.Enumerations;
using ISynergy.Framework.Monitoring.Messages;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace ISynergy.Framework.Monitoring.Client.Services
{
    /// <summary>
    /// Class ClientMonitorService.
    /// </summary>
    internal class ClientMonitorService : IClientMonitorService
    {
        /// <summary>
        /// The dialog service
        /// </summary>
        protected readonly IDialogService _dialogService;
        /// <summary>
        /// The language service
        /// </summary>
        protected readonly ILanguageService _languageService;
        /// <summary>
        /// The logger
        /// </summary>
        protected readonly ILogger _logger;
        /// <summary>
        /// The configuration options
        /// </summary>
        protected readonly ClientMonitorOptions _configurationOptions;

        /// <summary>
        /// The connection
        /// </summary>
        protected HubConnection _connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClientMonitorService"/> class.
        /// </summary>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="languageService">The language service.</param>
        /// <param name="configurationOptions">The configuration options.</param>
        /// <param name="logger">The logger factory.</param>
        public ClientMonitorService(
            IDialogService dialogService,
            ILanguageService languageService,
            IOptions<ClientMonitorOptions> configurationOptions,
            ILogger logger)
        {
            _dialogService = dialogService;
            _languageService = languageService;
            _configurationOptions = configurationOptions.Value;
            _logger = logger;
        }

        /// <summary>
        /// Connects the asynchronous.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <param name="connectionAction"></param>
        /// <returns>Task.</returns>
        public virtual Task ConnectAsync(string token, Action<HubConnection> connectionAction)
        {
            _logger.LogInformation($"Connecting to {_configurationOptions.EndpointUrl}");

            _connection = new HubConnectionBuilder()
                .WithUrl(_configurationOptions.EndpointUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .Build();

            _connection.On<HubMessage>(nameof(MonitorEvents.Connected), async (m) =>
            {
                // User has logged in. 
                await _dialogService.ShowInformationAsync(
                    string.Format(_languageService.GetString("Warning_User_Loggedin"), m.Data.ToString()));
            });

            _connection.On<HubMessage>(nameof(MonitorEvents.Disconnected), async (m) =>
            {
                // User has logged out. 
                await _dialogService.ShowInformationAsync(
                    string.Format(_languageService.GetString("Warning_User_Loggedout"), m.Data.ToString()));
            });

            // Set up additional handlers
            connectionAction.Invoke(_connection);

            return _connection.StartAsync();
        }

        /// <summary>
        /// disconnect as an asynchronous operation.
        /// </summary>
        public async Task DisconnectAsync()
        {
            if (_connection is not null)
                await _connection.DisposeAsync();
        }
    }
}
