using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Native;
using LlamaCpp.Net.Native.Models;
using LlamaCpp.Net.Samplers;
using LlamaCpp.Net.Samplers.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LlamaCpp.Net
{
    /// <summary>
    /// Represents a set of constraints that can be applied to token candidates based on the last tokens and inference options.
    /// </summary>
    internal sealed class ModelConstraints
    {
        private readonly SafeLLamaContextHandle _contextHandle;
        private readonly int _newLineToken;
        private readonly int _contextSize;


        public ModelConstraints(SafeLLamaContextHandle contextHandle)
        {
            _contextHandle = contextHandle;
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
        public void ApplyConstraints(TokenDataArray candidatesP, IEnumerable<int> lastTokens,
            Span<float> logits,
            InferenceOptions inferenceOptions)
        {
            var lt = lastTokens.ToList();
            var lastTokensCount = lt.Count;
            var lastNRepeat = Math.Min(Math.Min(lastTokensCount, inferenceOptions.RepeatLastTokensCount), _contextSize);


            var tokens = lt.Skip(lastTokensCount - lastNRepeat).ToArray();

            var samplers = new ISampler[]
            {
                RepetitionPenaltySampler.CreateInstance(_contextHandle, tokens,
                    inferenceOptions.RepetitionLastN, inferenceOptions.RepetitionPenalty),
                FrequencyAndPresencePenaltySampler.CreateInstance(_contextHandle, tokens,
                    inferenceOptions.RepetitionLastN, inferenceOptions.AlphaFrequency, inferenceOptions.AlphaPresence),
            };
            foreach (var sampler in samplers)
            {
                sampler.Sample(candidatesP);
            }


            var newLineLogit = logits[_newLineToken];

            if (!inferenceOptions.PenalizeNewLine)
            {
                logits[_newLineToken] = newLineLogit;
            }
        }
    }
}