﻿using ISynergy.Framework.Core.Abstractions.Services;
using System.IO;
using System.Reflection;

namespace ISynergy.Framework.Core.Services
{
    /// <summary>
    /// Class BaseInfoService.
    /// </summary>
    public class InfoService : IInfoService
    {
        /// <summary>
        /// The assembly
        /// </summary>
        private  readonly Assembly _assembly;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfoService" /> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public InfoService(Assembly assembly)
        {
            _assembly = assembly;
        }

        /// <summary>
        /// Gets the application path.
        /// </summary>
        /// <value>The application path.</value>
        public string ApplicationPath
        {
            get
            {
                return Path.GetDirectoryName(_assembly.Location);
            }
        }

        /// <summary>
        /// Gets the name of the company.
        /// </summary>
        /// <value>The name of the company.</value>
        public string CompanyName
        {
            get
            {
                return _assembly.GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;
            }
        }

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <value>The product version.</value>
        public string ProductVersion
        {
            get
            {
                if(_assembly.IsDefined(typeof(AssemblyInformationalVersionAttribute), false))
                {
                    return _assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
                }
                else if (_assembly.IsDefined(typeof(AssemblyVersionAttribute), false))
                {
                    return _assembly.GetCustomAttribute<AssemblyVersionAttribute>().Version;
                }
                else if (_assembly.IsDefined(typeof(AssemblyFileVersionAttribute), false))
                {
                    return _assembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
                }

                return "0.0.0";
            }
        }

        /// <summary>
        /// Gets the name of the product.
        /// </summary>
        /// <value>The name of the product.</value>
        public string ProductName
        {
            get
            {
                return _assembly.GetCustomAttribute<AssemblyProductAttribute>()?.Product;
            }
        }

        /// <summary>
        /// Gets the copy rights detail.
        /// </summary>
        /// <value>The copy rights detail.</value>
        public string Copyrights
        {
            get
            {
                return _assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()?.Copyright;
            }
        }
    }
}
