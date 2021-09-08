﻿using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Core.Extensions;
using ISynergy.Framework.Core.Locators;
using ISynergy.Framework.Core.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.ViewModels;
using ISynergy.Framework.UI;
using ISynergy.Framework.UI.Abstractions.Views;
using ISynergy.Framework.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Sample.Abstractions.Services;
using Sample.Options;
using Sample.Services;
using Sample.ViewModels;
using Sample.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.WebSockets;
using System.Reflection;
using System.Resources;

#if HAS_UNO
using Uno.Material;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
#endif

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

#if HAS_UNO
        /// <summary>
        /// On loanched event handler.
        /// </summary>
        /// <param name="args"></param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {

            // Set a default palette to make sure all colors used by MaterialResources exist
            //Application.Current.Resources.MergedDictionaries.Add(new MaterialColorPalette());

            // Add all the material resources. Those resources depend on the colors above, which is why this one must be added last.
            Application.Current.Resources.MergedDictionaries.Add(new MaterialResources());

            Application.Current.Resources["MaterialPrimaryColor"] = ThemeSelector.AccentColor;
            Application.Current.Resources["MaterialPrimaryVariantLightColor"] = ThemeSelector.LightAccentColor;
            Application.Current.Resources["MaterialPrimaryVariantDarkColor"] = ThemeSelector.DarkAccentColor;

            base.OnLaunched(args);
        }
#endif

        /// <summary>
        /// Configures the logger.
        /// </summary>
        /// <param name="factory">The factory.</param>
        protected override void ConfigureLogger(ILoggerFactory factory)
        {
            factory = LoggerFactory.Create(builder =>
            {
#if __WASM__
                builder.AddProvider(new global::Uno.Extensions.Logging.WebAssembly.WebAssemblyConsoleLoggerProvider());
#elif __IOS__
                builder.AddProvider(new global::Uno.Extensions.Logging.OSLogLoggerProvider());
#elif NETFX_CORE
                builder.AddDebug();
#else
                builder.AddConsole();
#endif

                // Exclude logs below this level
                builder.SetMinimumLevel(LogLevel.Information);

                // Default filters for Uno Platform namespaces
                builder.AddFilter("Uno", LogLevel.Warning);
                builder.AddFilter("Windows", LogLevel.Warning);
                builder.AddFilter("Microsoft", LogLevel.Warning);

                // Generic Xaml events
                builder.AddFilter("Microsoft.UI.Xaml", LogLevel.Debug);
                builder.AddFilter("Microsoft.UI.Xaml.VisualStateGroup", LogLevel.Debug);
                builder.AddFilter("Microsoft.UI.Xaml.StateTriggerBase", LogLevel.Debug);
                builder.AddFilter("Microsoft.UI.Xaml.UIElement", LogLevel.Debug);
                builder.AddFilter("Microsoft.UI.Xaml.FrameworkElement", LogLevel.Trace);

                // Layouter specific messages
                builder.AddFilter("Microsoft.UI.Xaml.Controls", LogLevel.Debug);
                builder.AddFilter("Microsoft.UI.Xaml.Controls.Layouter", LogLevel.Debug);
                builder.AddFilter("Microsoft.UI.Xaml.Controls.Panel", LogLevel.Debug);

                builder.AddFilter("Windows.Storage", LogLevel.Debug);

                // Binding related messages
                builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug);
                builder.AddFilter("Microsoft.UI.Xaml.Data", LogLevel.Debug);

                // Binder memory references tracking
                builder.AddFilter("Uno.UI.DataBinding.BinderReferenceHolder", LogLevel.Debug);

                // RemoteControl and HotReload related
                builder.AddFilter("Uno.UI.RemoteControl", LogLevel.Information);

                // Debug JS interop
                builder.AddFilter("Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug);
            });

#if HAS_UNO
            global::Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory = factory;
#endif
        }

        /// <summary>
        /// Configures the services.
        /// </summary>
        /// <param name="services">The services.</param>
        protected override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);

            var assembly = Assembly.GetAssembly(typeof(App));

            ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonStream(assembly.GetManifestResourceStream("appsettings.json"))
                .Build();

            services.Configure<ConfigurationOptions>(ConfigurationRoot.GetSection(nameof(ConfigurationOptions)).BindWithReload);

            services.AddSingleton<IInfoService>((s) => new InfoService(assembly));
            services.AddSingleton<IContext, Context>();
            services.AddSingleton<IAuthenticationService, AuthenticationService>();

#if NETFX_CORE
            services.AddSingleton<IUpdateService, UpdateService>();
#endif

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IBaseSettingsService, SettingsService>());
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ISettingsService, SettingsService>());

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
                assembly
            });
        }

        /// <summary>
        /// Handles the exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message.</param>
        public override void HandleException(Exception exception, string message)
        {
            try
            {
                Debug.WriteLine(message);

                if(exception.InnerException is WebSocketException)
                    return;

                // Set busyIndicator to false if it's true.
                ServiceLocator.Default.GetInstance<IBusyService>().EndBusy();

                if (exception is UnauthorizedAccessException accessException)
                {
                    ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(accessException.Message).Await();
                }
                else if (exception is IOException iOException)
                {
                    if (iOException.Message.Contains("The process cannot access the file") && iOException.Message.Contains("because it is being used by another process"))
                    {
                        ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(
                            ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_FILEINUSE")).Await();
                    }
                    else
                    {
                        ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(iOException.Message).Await();
                    }
                }
                else if (exception is ArgumentException argumentException)
                {
                    ServiceLocator.Default.GetInstance<IDialogService>().ShowWarningAsync(
                        string.Format(
                            ServiceLocator.Default.GetInstance<ILanguageService>().GetString("EX_ARGUMENTNULL"),
                            argumentException.ParamName)
                        ).Await();
                }
                else
                {
                    ServiceLocator.Default.GetInstance<IDialogService>().ShowErrorAsync(exception.Message).Await();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message, ex);
            }
        }
    }
}
