﻿using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Telemetry.Options;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ISynergy.Framework.Telemetry.Services
{
    /// <summary>
    /// See https://www.meziantou.net/use-application-insights-in-a-desktop-application.htm
    /// </summary>
    internal class TelemetryService : ITelemetryService
    {
        /// <summary>
        /// The application insights options
        /// </summary>
        private readonly ApplicationInsightsOptions _applicationInsightsOptions;

        /// <summary>
        /// Telemetry client for Application Insights.
        /// </summary>
        private TelemetryClient Client { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryService"/> class.
        /// </summary>
        /// <param name="infoService">The information service.</param>
        /// <param name="options"></param>
        public TelemetryService(IInfoService infoService, IOptions<ApplicationInsightsOptions> options)
        {
            _applicationInsightsOptions = options.Value;

            var config = new TelemetryConfiguration(_applicationInsightsOptions.Key);
            Client = new TelemetryClient(config);
            Client.Context.User.UserAgent = infoService.ProductName;
            Client.Context.Component.Version = infoService.ProductVersion.ToString();
            Client.Context.Session.Id = Guid.NewGuid().ToString();
            Client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public void Flush()
        {
            Client?.Flush();
        }

        /// <summary>
        /// Tracks the event asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public Task TrackEventAsync(string e)
        {
            Client.TrackEvent(e);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Tracks the event asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <param name="props">The props.</param>
        /// <returns>Task.</returns>
        public Task TrackEventAsync(string e, Dictionary<string, string> props)
        {
            Client.TrackEvent(e, props, null);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Tracks the exception asynchronous.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="message">The message.</param>
        /// <returns>Task.</returns>
        public Task TrackExceptionAsync(Exception ex, string message)
        {
            if (ex is not null)
            {
                Client.TrackException(new ExceptionTelemetry { Exception = ex, Message = message });
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Tracks the page view asynchronous.
        /// </summary>
        /// <param name="e">The e.</param>
        /// <returns>Task.</returns>
        public Task TrackPageViewAsync(string e)
        {
            Client.TrackPageView(e);
            return Task.CompletedTask;
        }
    }
}
