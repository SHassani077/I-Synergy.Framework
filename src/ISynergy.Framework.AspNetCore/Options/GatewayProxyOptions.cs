﻿using ISynergy.Framework.AspNetCore.Models;
using System.Collections.Generic;

namespace ISynergy.Framework.AspNetCore.Options
{
    /// <summary>
    /// Options for gateway proxies.
    /// </summary>
    public class GatewayProxyOptions
    {
        /// <summary>
        /// List of <see cref="Proxy" />.
        /// </summary>
        /// <value>The gateway proxies.</value>
        public List<Proxy> GatewayProxies { get; set; }
    }
}
