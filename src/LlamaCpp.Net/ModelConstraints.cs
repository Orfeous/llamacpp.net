using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Native;
using LlamaCpp.Net.Native.Models;
using LlamaCpp.Net.Samplers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LlamaCpp.Net
{
    /// <summary>
    /// Represents a set of constraints that can be applied to token candidates based on the last tokens and inference options.
    /// </summary>
    internal sealed unsafe class ModelConstraints
    {
        private readonly SafeLLamaContextHandle _contextHandle;
        private readonly ILogger _logger;
        private readonly int _newLineToken;
        private readonly int _contextSize;


        public ModelConstraints(SafeLLamaContextHandle contextHandle, ILogger logger)
        {
            _contextHandle = contextHandle;
            _logger = logger;
            _newLineToken = LlamaNative.llama_token_nl();
            _contextSize = _contextHandle.llama_n_ctx();
        }


        /// <summary>
        /// Applies constraints to the given token candidates based on the last tokens and inference options.
        /// </summary>
        /// <param name="candidatesP">The token candidates to apply constraints to.</param>
        /// <param name="lastTokens">The last tokens used in the model.</param>
        /// <param name="logits">The logits for each token.</param>
        /// <param name="inferenceOptions">The inference options to use for applying constraints.</param>
        public int ApplyConstraints(TokenDataArray candidatesP, IEnumerable<int> lastTokens,
            Span<float> logits,
            InferenceOptions inferenceOptions)
        {
            var lt = lastTokens.ToList();
            var lastTokensCount = lt.Count;
            // take lastTokenCount, inferenceOptions.TokenLookback or _contextSize,
            // whichever is the smallest
            // this gets used to determine how far to look back for the repetition penalty
            var lastNRepeat = Math.Min(Math.Min(lastTokensCount, inferenceOptions.TokenLookback), _contextSize);


            var tokens = lt.Skip(lastTokensCount - lastNRepeat).ToArray();

            // Pin the data array to prevent the garbage collector from moving it around
            var handle = candidatesP.data.Pin();

            // Create a native representation of the token data array
            var st = new TokenDataArrayNative
            {
                data = new IntPtr(handle.Pointer),
                size = candidatesP.size,
                sorted = candidatesP.sorted
            };
            var ptr = new IntPtr(&st);

            /*
            var samplers = new ISampler[]
            {

                new RepetitionPenaltySampler(_contextHandle, tokens,
                    inferenceOptions),

                new FrequencyAndPresencePenaltySampler(_contextHandle, tokens,
                    inferenceOptions.RepetitionLastN, inferenceOptions.AlphaFrequency, inferenceOptions.AlphaPresence),
            };
            foreach (var sampler in samplers)
            {
                sampler.Sample(ptr);
            }
            */


            var newLineLogit = logits[_newLineToken];

            if (!inferenceOptions.PenalizeNewLine)
            {
                logits[_newLineToken] = newLineLogit;
            }

            var mu = 0.0f;

            new TemperatureSampler(_contextHandle, inferenceOptions).Sample(ptr);
            return inferenceOptions.SamplingMethod switch
            {
                SamplingMethod.Mirostat => _contextHandle.llama_sample_token_mirostat(ptr, 1, 1, 100, &mu),
                SamplingMethod.MirostatV2 => _contextHandle.llama_sample_token_mirostat_v2(ptr, 1, 1, &mu),
                _ => _contextHandle.llama_sample_token(ptr)
            };
        }

    }
}