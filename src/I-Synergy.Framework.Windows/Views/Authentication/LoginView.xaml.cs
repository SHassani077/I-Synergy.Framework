﻿using GalaSoft.MvvmLight.Ioc;
using ISynergy.ViewModels.Authentication;
using Windows.UI.Xaml.Navigation;

namespace ISynergy.Views.Authentication
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginView : ILoginView
    {
        private ILoginViewModel ViewModel => SimpleIoc.Default.GetInstance<ILoginViewModel>();

        public LoginView()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if(ViewModel.Context.IsAuthenticated)
            {
                await ViewModel.BaseService.LoginService.LogoutAsync();
            }

            await ViewModel.CheckAutoLogin();
        }
    }
}
