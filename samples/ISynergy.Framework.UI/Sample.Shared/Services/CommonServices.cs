﻿using ISynergy.Framework.UI.Services;
using ISynergy.Framework.Core.Abstractions.Services;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Sample.Abstractions.Services;

namespace Sample.Services
{
    /// <summary>
    /// Class CommonServices.
    /// </summary>
    public class CommonServices : BaseCommonService, ICommonServices
    {
        /// <summary>
        /// Gets the authentication service.
        /// </summary>
        /// <value>The authentication service.</value>
        public IAuthenticationService AuthenticationService { get; }
        /// <summary>
        /// Gets the file service.
        /// </summary>
        /// <value>The file service.</value>
        public IFileService FileService { get; }
        /// <summary>
        /// Gets the clipboard service.
        /// </summary>
        /// <value>The clipboard service.</value>
        public IClipboardService ClipboardService { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonServices" /> class.
        /// </summary>
        /// <param name="busyService">The busy indicator service.</param>
        /// <param name="messageService">The messaging service</param>
        /// <param name="languageService">The language service.</param>
        /// <param name="telemetryService">The telemetry service.</param>
        /// <param name="dialogService">The dialog service.</param>
        /// <param name="navigationService">The navigation service.</param>
        /// <param name="fileService">The file service.</param>
        /// <param name="infoService">The information service.</param>
        /// <param name="converterService">The converter service.</param>
        /// <param name="dispatcherService">The dispatcher service.</param>
        /// <param name="authenticationService">The authentication service.</param>
        /// <param name="clipboardService">The clipboard service.</param>
        public CommonServices(
            IBusyService busyService,
            IMessageService messageService,
            ILanguageService languageService,
            ITelemetryService telemetryService,
            IDialogService dialogService,
            INavigationService navigationService,
            IFileService fileService,
            IInfoService infoService,
            IConverterService converterService,
            IDispatcherService dispatcherService,
            IAuthenticationService authenticationService,
            IClipboardService clipboardService)
            :base(busyService,
                 messageService,
                 languageService, 
                 telemetryService, 
                 dialogService, 
                 navigationService, 
                 infoService, 
                 converterService,
                 dispatcherService)
        {
            AuthenticationService = authenticationService;
            FileService = fileService;
            ClipboardService = clipboardService;
        }
    }
}
