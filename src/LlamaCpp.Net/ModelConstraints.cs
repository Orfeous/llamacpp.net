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


        public void ApplyConstraints(TokenDataArray candidatesP, IEnumerable<int> lastTokens,
            Span<float> logits,
            InferenceOptions inferenceOptions,
            float alphaFrequency = .0f,
            float alphaPresence = .0f)
        {
            var lt = lastTokens.ToList();
            var lastTokensCount = lt.Count;
            var lastNRepeat = Math.Min(Math.Min(lastTokensCount, inferenceOptions.RepeatLastTokensCount), _contextSize);


            var tokens = lt.Skip(lastTokensCount - lastNRepeat).ToArray();

            var samplers = new ISampler[]
            {
                new RepetitionPenaltySampler(_contextHandle, tokens,
                    (ulong)lastNRepeat, inferenceOptions.RepetitionPenalty),
                new FrequencyAndPresencePenaltySampler(_contextHandle, tokens,
                    (ulong)lastNRepeat, alphaFrequency, alphaPresence)
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