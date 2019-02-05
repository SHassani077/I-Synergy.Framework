﻿using ISynergy.Mvvm;
using System;
using System.Reflection;

namespace ISynergy.Locators
{
    /// <summary>
    /// Dialog type locator responsible for locating dialog types for specified view models based
    /// on a naming convention used in a multitude of articles and code samples regarding the MVVM
    /// pattern.
    /// <para/>
    /// The convention states that if the name of the view model is
    /// 'MyNamespace.ViewModels.MyDialogViewModel' then the name of the dialog is
    /// 'MyNamespace.Views.MyDialog'.
    /// </summary>
    public class DialogTypeLocator : IDialogTypeLocator
    {
        internal static readonly DialogTypeLocatorCache Cache = new DialogTypeLocatorCache();

        /// <summary>
        /// Locates the dialog type representing the specified view model in a user interface.
        /// </summary>
        /// <param name="viewModel">The view model to find the dialog type for.</param>
        /// <returns>
        /// The dialog type representing the specified view model in a user interface.
        /// </returns>
        public Type Locate(IViewModel viewModel)
        {
            if (viewModel is null)
                throw new ArgumentNullException(nameof(viewModel));

            Type viewModelType = viewModel.GetType();

            Type dialogType = Cache.Get(viewModelType);

            if (dialogType != null)
            {
                return dialogType;
            }

            string dialogName = GetDialogName(viewModelType);

            dialogType = GetAssemblyFromType(viewModelType).GetType(dialogName);

            if (dialogType is null)
                throw new TypeLoadException(AppendInfoAboutDialogTypeLocators($"Dialog with name '{dialogName}' is missing."));

            Cache.Add(viewModelType, dialogType);

            return dialogType;
        }

        private static string GetDialogName(Type viewModelType)
        {
            string dialogName = viewModelType.FullName.Replace(".ViewModels.", ".Views.");

            if (!dialogName.EndsWith("ViewModel", StringComparison.Ordinal))
                throw new TypeLoadException(AppendInfoAboutDialogTypeLocators($"View model of type '{viewModelType}' doesn't follow naming convention since it isn't suffixed with 'ViewModel'."));

            return dialogName.Substring(
                0,
                dialogName.Length - "ViewModel".Length);
        }

        private static Assembly GetAssemblyFromType(Type type) => type.Assembly;

        private static string AppendInfoAboutDialogTypeLocators(string errorMessage)
        {
            return
                errorMessage + Environment.NewLine +
                "If your project structure doesn't conform to the default convention of MVVM " +
                "Dialogs you can always define a new convention by implementing your own dialog " +
                "type locator. For more information on how to do that, please read the GitHub " +
                "wiki or ask the author.";
        }
    }
}
