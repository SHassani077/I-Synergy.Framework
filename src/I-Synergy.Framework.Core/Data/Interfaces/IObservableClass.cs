﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ISynergy
{
    public interface IObservableClass : IBindable
    {
        bool Validate(bool validateAfter);
        void Revert();
        void MarkAsClean();
        ObservableConcurrentDictionary<string, IProperty> Properties { get; }
        ObservableCollection<string> Errors { get; }
        Action<IObservableClass> Validator { set; get; }
        bool IsValid { get; }
        bool IsDirty { get; }
    }
}
