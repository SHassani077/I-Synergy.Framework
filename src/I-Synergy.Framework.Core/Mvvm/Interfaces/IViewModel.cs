﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ISynergy.Services;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ISynergy.Mvvm
{
    public class SubmittedItemArgs : EventArgs
    {
        public IViewModel Result { get; set; }
    }

    public interface IViewModel : IObservableClass, ICleanup
    {
        IContext Context { get; }
        IBaseService BaseService { get; }

        RelayCommand Close_Command { get; }

        bool CanClose { get; set; }
        bool IsCancelled { get; }
        string Title { get; }
        Task OnDeactivateAsync();
        Task OnActivateAsync(object parameter, bool isBack);
        Task InitializeAsync();
        void OnPropertyChanged(object sender, PropertyChangedEventArgs e);
    }
}