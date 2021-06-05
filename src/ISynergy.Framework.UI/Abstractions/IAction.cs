﻿namespace ISynergy.Framework.UI.Abstractions
{
    /// <summary>
    /// Interface implemented by all custom actions.
    /// </summary>
    public interface IAction
    {
        /// <summary>
        /// Executes the action.
        /// </summary>
        /// <param name="sender">The <see cref="System.Object"/> that is passed to the action by the behavior.</param>
        /// <param name="parameter">The value of this parameter is determined by the caller.</param>
        /// <remarks> An example of parameter usage is EventTriggerBehavior, which passes the EventArgs as a parameter to its actions.</remarks>
        /// <returns>Returns the result of the action.</returns>
        object Execute(object sender, object parameter);
    }
}
