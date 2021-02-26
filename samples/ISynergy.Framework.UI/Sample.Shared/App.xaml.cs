﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI.Abstractions.Views;
using Sample.Abstractions.Services;
using Sample.Options;
using ISynergy.Framework.UI.Services;
using Sample.Services;
using Sample.ViewModels;
using Sample.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using System.Resources;
using System.Runtime.CompilerServices;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI;
using ISynergy.Framework.Core.Locators;
using System.IO;
using ISynergy.Framework.Core.Exceptions;
using System.Net.WebSockets;
using ISynergy.Framework.UI;
using Windows.ApplicationModel;
using ISynergy.Framework.Core.Services;
using System.Linq;

namespace Sample
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : BaseApplication
    {
        /// <summary>
        /// The configuration root
        /// </summary>
        private IConfigurationRoot ConfigurationRoot;

        /// <summary>
        /// The application center options
        /// </summary>
        private AppCenterOptions _appCenterOptions = new AppCenterOptions();

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App() : base() 
        {
            InitializeComponent();
        }

#if NETFX_CORE
        protected override IList<ResourceDictionary> GetAdditionalResourceDictionaries() =>
            new List<ResourceDictionary>()
            {
                new Microsoft.UI.Xaml.Controls.XamlControlsResources()
            };
#endif

        protected override void ConfigureLogger(ILoggerFactory factory)
        {
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            var assembly = Assembly.GetAssembly(typeof(App));

            ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonStream(assembly.GetManifestResourceStream("appsettings.json"))
                .Build();

            services.AddSingleton((s) => ConfigurationRoot.GetSection(nameof(ConfigurationOptions)).Get<ConfigurationOptions>());

            services.AddSingleton<IInfoService>((s) => new InfoService(assembly));
            services.AddSingleton<IContext, Context>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();

#if NETFX_CORE
            services.AddSingleton<IUpdateService, UpdateService>();
#endif

            services.AddSingleton<ISettingsService, SettingsService>();

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseCommonServices, CommonServices>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ICommonServices, CommonServices>());

            services.AddSingleton<ITelemetryService, TelemetryService>();
            services.AddSingleton<IMasterDataService, MasterDataService>();
            services.AddSingleton<IClientMonitorService, ClientMonitorService>();
            services.AddSingleton<ICameraService, CameraService>();
            services.AddSingleton<IClipboardService, ClipboardService>();
            services.AddSingleton<IDownloadFileService, DownloadFileService>();
            services.AddSingleton<IReportingService, ReportingService>();
            services.AddSingleton<IPrintingService, PrintingService>();

            services.AddSingleton<IShellViewModel, ShellViewModel>();
            services.AddSingleton<IShellView, ShellView>();

            LanguageService.AddResourceManager(new ResourceManager(typeof(Resources)));

            //Load assemblies
            RegisterAssemblies(new List<Assembly>
                {
                    assembly,
                    Assembly.GetAssembly(typeof(Sample.Core.Assembly.CoreIdentifier)),
                });
        }

        public override async Task HandleException(Exception exception, string message)
        {
            try
            {
                Debug.WriteLine(message);

                if(exception.InnerException is WebSocketException)
                    return;

                // Set busyIndicator to false if it's true.
                await ServiceLocator.Default.GetInstance<IBusyService>().EndBusyAsync();

                if (exception is UnauthorizedAccessException accessException)
                {
                    await ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(accessException.Message);
                }
                else if (exception is IOException iOException)
                {
                    if (iOException.Message.Contains("The process cannot access the file") && iOException.Message.Contains("because it is being used by another process"))
                    {
                        await ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(
                            ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_FILEINUSE"));
                    }
                    else
                    {
                        await ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(iOException.Message);
                    }
                }
                else if (exception is ArgumentException argumentException)
                {
                    await ServiceLocator.Default.GetInstance<IDialogService>().ShowWarningAsync(
                        string.Format(
                            ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_ARGUMENTNULL"),
                            argumentException.ParamName)
                        );
                }
                else
                {
                    await ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(exception.Message);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message, ex);
            }
        }
    }
}
