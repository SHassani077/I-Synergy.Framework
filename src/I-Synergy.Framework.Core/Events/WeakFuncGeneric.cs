﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace ISynergy.Framework.Core.Events
{
    /// <summary>
    /// Stores an Func without causing a hard reference
    /// to be created to the Func's owner. The owner can be garbage collected at any time.
    /// </summary>
    /// <typeparam name="T">The type of the Func's parameter.</typeparam>
    /// <typeparam name="TResult">The type of the Func's return value.</typeparam>
    public class WeakFunc<T, TResult> : WeakFunc<TResult>, IExecuteWithObjectAndResult
    {
        /// <summary>
        /// The static function
        /// </summary>
        private Func<T, TResult> _staticFunc;

        /// <summary>
        /// Gets or sets the name of the method that this WeakFunc represents.
        /// </summary>
        /// <value>The name of the method.</value>
        public override string MethodName
        {
            get
            {
                if (_staticFunc != null)
                {
                    return _staticFunc.Method.Name;
                }

                return Method.Name;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Func's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        /// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
        public override bool IsAlive
        {
            get
            {
                if (_staticFunc == null
                    && Reference == null)
                {
                    return false;
                }

                if (_staticFunc != null)
                {
                    if (Reference != null)
                    {
                        return Reference.IsAlive;
                    }

                    return true;
                }

                return Reference.IsAlive;
            }
        }

        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="func">The Func that will be associated to this instance.</param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is using closures.</param>
        public WeakFunc(Func<T, TResult> func, bool keepTargetAlive = false)
            : this(func == null ? null : func.Target, func, keepTargetAlive)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="target">The Func's owner.</param>
        /// <param name="func">The Func that will be associated to this instance.</param>
        /// <param name="keepTargetAlive">If true, the target of the Action will
        /// be kept as a hard reference, which might cause a memory leak. You should only set this
        /// parameter to true if the action is using closures.</param>
        [SuppressMessage(
            "Microsoft.Design", 
            "CA1062:Validate arguments of public methods",
            MessageId = "1",
            Justification = "Method should fail with an exception if func is null.")]
        public WeakFunc(object target, Func<T, TResult> func, bool keepTargetAlive = false)
        {
            if (func.Method.IsStatic)
            {
                _staticFunc = func;

                if (target != null)
                {
                    // Keep a reference to the target to control the
                    // WeakAction's lifetime.
                    Reference = new WeakReference(target);
                }

                return;
            }

            Method = func.Method;
            FuncReference = new WeakReference(func.Target);
            LiveReference = keepTargetAlive ? func.Target : null;
            Reference = new WeakReference(target);

#if DEBUG
            if (FuncReference != null
                && FuncReference.Target != null
                && !keepTargetAlive)
            {
                var type = FuncReference.Target.GetType();

                if (type.Name.StartsWith("<>")
                    && type.Name.Contains("DisplayClass"))
                {
                    System.Diagnostics.Debug.WriteLine(
                        "You are attempting to register a lambda with a closure without using keepTargetAlive. Are you sure? Check http://galasoft.ch/s/mvvmweakaction for more info.");
                }
            }
#endif
        }

        /// <summary>
        /// Executes the Func. This only happens if the Func's owner
        /// is still alive. The Func's parameter is set to default(T).
        /// </summary>
        /// <returns>The result of the Func stored as reference.</returns>
        public new TResult Execute()
        {
            return Execute(default(T));
        }

        /// <summary>
        /// Executes the Func. This only happens if the Func's owner
        /// is still alive.
        /// </summary>
        /// <param name="parameter">A parameter to be passed to the action.</param>
        /// <returns>The result of the Func stored as reference.</returns>
        public TResult Execute(T parameter)
        {
            if (_staticFunc != null)
            {
                return _staticFunc(parameter);
            }

            var funcTarget = FuncTarget;
            
            if (IsAlive)
            {
                if (Method != null
                    && (LiveReference != null
                        || FuncReference != null)
                    && funcTarget != null)
                {
                    return (TResult) Method.Invoke(
                        funcTarget,
                        new object[]
                        {
                            parameter
                        });
                }
            }

            return default(TResult);
        }

        /// <summary>
        /// Executes the Func with a parameter of type object. This parameter
        /// will be casted to T. This method implements <see cref="IExecuteWithObject.ExecuteWithObject" />
        /// and can be useful if you store multiple WeakFunc{T} instances but don't know in advance
        /// what type T represents.
        /// </summary>
        /// <param name="parameter">The parameter that will be passed to the Func after
        /// being casted to T.</param>
        /// <returns>The result of the execution as object, to be casted to T.</returns>
        public object ExecuteWithObject(object parameter)
        {
            var parameterCasted = (T)parameter;
            return Execute(parameterCasted);
        }

        /// <summary>
        /// Sets all the funcs that this WeakFunc contains to null,
        /// which is a signal for containing objects that this WeakFunc
        /// should be deleted.
        /// </summary>
        public new void MarkForDeletion()
        {
            _staticFunc = null;
            base.MarkForDeletion();
        }
    }
}
