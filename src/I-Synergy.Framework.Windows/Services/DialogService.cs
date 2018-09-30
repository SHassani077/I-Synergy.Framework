﻿using ISynergy.Enumerations;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace ISynergy.Services
{
    /// <summary>
    /// Class abstracting the interaction between view models and views when it comes to
    /// opening dialogs using the MVVM pattern in UWP applications.
    /// </summary>
    public class DialogService : IDialogService
    {
        public ILanguageService LanguageService { get; }

        public DialogService(ILanguageService languageService)
        {
            LanguageService = languageService;
        }

        public Task<MessageBoxResult> ShowErrorAsync(Exception error, string title = "")
        {
            return ShowAsync(error.Message, title != "" ? title : LanguageService.GetString("TitleError"), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public Task<MessageBoxResult> ShowErrorAsync(string message, string title = "")
        {
            return ShowAsync(message, title != "" ? title : LanguageService.GetString("TitleError"), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public Task<MessageBoxResult> ShowInformationAsync(string message, string title = "")
        {
            return ShowAsync(message, title != "" ? title : LanguageService.GetString("TitleInfo"), MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public Task<MessageBoxResult> ShowWarningAsync(string message, string title = "")
        {
            return ShowAsync(message, title != "" ? title : LanguageService.GetString("TitleWarning"), MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public Task ShowGreetingAsync(string name)
        {
            if(DateTime.Now.Hour >= 0 && DateTime.Now.Hour < 6)
            {
                return ShowAsync(string.Format(LanguageService.GetString("Generic_Greeting_Night"), name),
                    LanguageService.GetString("TitleWelcome"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour < 12)
            {
                return ShowAsync(string.Format(LanguageService.GetString("Generic_Greeting_Morning"), name),
                    LanguageService.GetString("TitleWelcome"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (DateTime.Now.Hour >= 12 && DateTime.Now.Hour < 18)
            {
                return ShowAsync(string.Format(LanguageService.GetString("Generic_Greeting_Afternoon"), name),
                    LanguageService.GetString("TitleWelcome"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                return ShowAsync(string.Format(LanguageService.GetString("Generic_Greeting_Evening"), name),
                    LanguageService.GetString("TitleWelcome"), MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public virtual async Task<MessageBoxResult> ShowAsync(
            string message, 
            string title = "", 
            MessageBoxButton buttons = MessageBoxButton.OK, 
            MessageBoxImage image = MessageBoxImage.Information)
        {
            var yesCommand = new UICommand(LanguageService.GetString("Generic_Yes"), cmd => { });
            var noCommand = new UICommand(LanguageService.GetString("Generic_No"), cmd => { });
            var okCommand = new UICommand(LanguageService.GetString("Generic_Ok"), cmd => { });
            var cancelCommand = new UICommand(LanguageService.GetString("Generic_Cancel"), cmd => { });

            var messageDialog = new MessageDialog(message, title)
            {
                Options = MessageDialogOptions.None,
                DefaultCommandIndex = 0,
                CancelCommandIndex = 0
            };

            switch (buttons)
            {
                case MessageBoxButton.OK:
                    messageDialog.Commands.Add(okCommand);
                    messageDialog.DefaultCommandIndex = 0;
                    break;
                case MessageBoxButton.OKCancel:
                    messageDialog.Commands.Add(okCommand);
                    messageDialog.Commands.Add(cancelCommand);

                    messageDialog.DefaultCommandIndex = 0;
                    messageDialog.CancelCommandIndex = 1;
                    break;
                case MessageBoxButton.YesNoCancel:
                    messageDialog.Commands.Add(yesCommand);
                    messageDialog.Commands.Add(noCommand);
                    messageDialog.Commands.Add(cancelCommand);

                    messageDialog.DefaultCommandIndex = 0;
                    messageDialog.CancelCommandIndex = 2;
                    break;
                case MessageBoxButton.YesNo:
                    messageDialog.Commands.Add(yesCommand);
                    messageDialog.Commands.Add(noCommand);

                    messageDialog.DefaultCommandIndex = 0;
                    messageDialog.CancelCommandIndex = 1;
                    break;
            }

            var command = await messageDialog.ShowAsync();

            if(command is null && cancelCommand != null)
            {
                // back button was pressed
                // invoke the UICommand

                cancelCommand.Invoked(cancelCommand);
                return MessageBoxResult.Cancel;
            }

            if (command == okCommand)
                return MessageBoxResult.OK;
            else if (command == yesCommand)
                return MessageBoxResult.Yes;
            else if (command == noCommand)
                return MessageBoxResult.No;

            return MessageBoxResult.Cancel;
        }
    }
}
