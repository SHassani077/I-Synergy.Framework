﻿using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ISynergy.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ISynergy.Events;
using System.Collections;
using ISynergy.Helpers;
using System.Linq;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace ISynergy.Mvvm
{
    public abstract class ViewModel : ObservableClass, IViewModel
    {
        public delegate Task Submit_Action(object e);

        public IContext Context { get; }
        public IBaseService BaseService { get; }

        public RelayCommand Close_Command { get; protected set; }

        /// <summary>
        /// Gets or sets the Title property value.
        /// </summary>
        public virtual string Title
        {
            get { return GetValue<string>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the NumberInfo property value.
        /// </summary>
        public NumberFormatInfo NumberFormat
        {
            get { return GetValue<NumberFormatInfo>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Culture property value.
        /// </summary>
        public CultureInfo Culture
        {
            get { return GetValue<CultureInfo>(); }
            set { SetValue(value); }
        }
        
        /// <summary>
        /// Gets or sets the IsInitialized property value.
        /// </summary>
        public bool IsInitialized
        {
            get { return GetValue<bool>(); }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the Mode_IsAdvanced property value.
        /// </summary>
        public bool Mode_IsAdvanced
        {
            get { return BaseService.BaseSettingsService.Application_Advanced; }
            set
            {
                BaseService.BaseSettingsService.Application_Advanced = value;

                if (value)
                {
                    Mode_ToolTip = BaseService.LanguageService.GetString("Generic_Advanced");
                }
                else
                {
                    Mode_ToolTip = BaseService.LanguageService.GetString("Generic_Basic");
                }
            }
        }

        /// <summary>
        /// Gets or sets the Mode_ToolTip property value.
        /// </summary>
        public string Mode_ToolTip
        {
            get
            {
                string result = GetValue<string>();

                if (string.IsNullOrWhiteSpace(result))
                {
                    if (Mode_IsAdvanced)
                    {
                        result = BaseService.LanguageService.GetString("Generic_Advanced");
                    }
                    else
                    {
                        result = BaseService.LanguageService.GetString("Generic_Basic");
                    }
                }

                return result;
            }
            set { SetValue(value); }
        }

        private WeakEventListener<IViewModel, object, PropertyChangedEventArgs> WeakViewModelPropertyChangedEvent = null;

        public ViewModel(
            IContext context, 
            IBaseService baseService)
            : base()
        {
            Context = context;
            BaseService = baseService;

            WeakViewModelPropertyChangedEvent = new WeakEventListener<IViewModel, object, PropertyChangedEventArgs>(this)
            {
                OnEventAction = (instance, source, eventargs) => instance.OnPropertyChanged(source, eventargs),
                OnDetachAction = (listener) => this.PropertyChanged -= listener.OnEvent
            };

            this.PropertyChanged += WeakViewModelPropertyChangedEvent.OnEvent;

            Messenger.Default.Register<ExceptionHandledMessage>(this, i => BaseService.BusyService.EndBusyAsync());

            Culture = Thread.CurrentThread.CurrentCulture;
            Culture.NumberFormat.CurrencySymbol = $"{Context.CurrencySymbol} ";
            Culture.NumberFormat.CurrencyNegativePattern = 1;

            NumberFormat = Culture.NumberFormat;
            
            IsInitialized = false;

            Close_Command = new RelayCommand(() =>
            {
                Messenger.Default.Send(new OnCancelMessage(this));
            });
        }

        public virtual async Task InitializeAsync()
        {
            await BaseService.TelemetryService.TrackPageViewAsync(GetType().Name.Replace("ViewModel", ""));
        }

        protected string GetEnumDescription(Enum value)
        {
            Argument.IsNotNull(nameof(value), value);

            string description = value.ToString();
            FieldInfo fieldInfo = value.GetType().GetField(description);
            DisplayAttribute[] attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                description = BaseService.LanguageService.GetString(attributes[0].Description);
            }

            return description;
        }

        public virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        public bool CanClose { get; set; }
        public bool IsCancelled { get; protected set; }

        protected virtual Task CancelViewModelAsync()
        {
            IsCancelled = true;
            return Task.CompletedTask;
        }

        public virtual Task OnDeactivateAsync()
        {
            Cleanup();
            return Task.CompletedTask;
        }

        public virtual Task OnActivateAsync(object parameter, bool isBack) => InitializeAsync();

        public virtual void Cleanup()
        {
            Messenger.Default.Unregister(this);
        }
    }
}