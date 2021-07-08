﻿using ISynergy.Framework.Core.Abstractions;
using ISynergy.Framework.Core.Enumerations;
using ISynergy.Framework.Mvvm;
using ISynergy.Framework.Mvvm.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace Sample.ViewModels
{
    /// <summary>
    /// Class ConvertersViewModel.
    /// </summary>
    public class ConvertersViewModel : ViewModelNavigation<object>
    {
        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get { return BaseCommonServices.LanguageService.GetString("Converters"); } }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConvertersViewModel"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="commonServices">The common services.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public ConvertersViewModel(
            IContext context,
            IBaseCommonServices commonServices,
            ILoggerFactory loggerFactory)
            : base(context, commonServices, loggerFactory)
        {
            SelectedSoftwareEnvironment = (int)SoftwareEnvironments.Production;
        }

        /// <summary>
        /// Gets or sets the SoftwareEnvironments property value.
        /// </summary>
        /// <value>The software environments.</value>
        public SoftwareEnvironments SoftwareEnvironments
        {
            get { return GetValue<SoftwareEnvironments>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the SelectedSoftwareEnvironment property value.
        /// </summary>
        /// <value>The selected software environment.</value>
        public int SelectedSoftwareEnvironment
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

    }
}
