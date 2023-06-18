﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using LlamaCpp.Net.Configuration;

namespace LlamaCpp.Net.Abstractions
{
    internal interface ILanguageModel : IDisposable
    {
        /// <summary>
        /// The path to the model
        /// </summary>
        string ModelPath { get; }

        /// <summary>
        /// The options for the language model
        /// </summary>
        LanguageModelOptions Options { get; }
    }
}
