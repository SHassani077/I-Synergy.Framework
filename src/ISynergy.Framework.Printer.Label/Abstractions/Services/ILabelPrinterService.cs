﻿using System.Threading.Tasks;

namespace ISynergy.Framework.Printer.Label.Abstractions.Services
{
    /// <summary>
    /// Label printer interface.
    /// </summary>
    public interface ILabelPrinterService
    {
        /// <summary>
        /// Print label async
        /// </summary>
        /// <param name="content"></param>
        /// <param name="copies"></param>
        Task PrintLabelAsync(string content, int copies = 1);
    }
}
