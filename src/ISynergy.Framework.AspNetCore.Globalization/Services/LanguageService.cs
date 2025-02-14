﻿using System;
using System.Collections.Generic;

namespace ISynergy.Framework.AspNetCore.Globalization.Services
{
    /// <summary>
    /// Class LanguageService.
    /// Implements the <see cref="ILanguageService" />
    /// </summary>
    /// <seealso cref="ILanguageService" />
    internal class LanguageService : ILanguageService
    {
        /// <summary>
        /// The managers
        /// </summary>
        private readonly List<ResourceManager> _managers;

        /// <summary>
        /// The HTTP context accessor
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageService" /> class.
        /// </summary>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public LanguageService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _managers = new List<ResourceManager>();
        }

        /// <summary>
        /// Adds the resource manager.
        /// </summary>
        /// <param name="resourceManager">The resource manager.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void AddResourceManager(ResourceManager resourceManager) =>
            _managers.Add(resourceManager);

        /// <summary>
        /// Gets the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>System.String.</returns>
        public string GetString(string key)
        {
            var languages = _httpContextAccessor.HttpContext.Request.GetTypedHeaders().AcceptLanguage;

            foreach (var manager in _managers)
            {
                var result = manager.GetString(key, CultureInfo.CurrentCulture);

                if (string.IsNullOrEmpty(result))
                    result = manager.GetString(key);

                if (!string.IsNullOrEmpty(result))
                    return result;
            }

            return $"[{key}]";
        }
    }
}
