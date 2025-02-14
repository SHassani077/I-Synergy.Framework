﻿using ISynergy.Framework.Update.Abstractions.Services;
using ISynergy.Framework.Update.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ISynergy.Framework.Update.Extensions
{
    /// <summary>
    /// Service collection extensions for Microsoft Stor updates
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds update integration.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddUpdatesIntegration(this IServiceCollection services)
        {
            services.AddSingleton<IUpdateService, UpdateService>();
            return services;
        }
    }
}
