using System;
using System.Collections.Generic;
using System.Linq;
using LlamaCpp.Net.Native;

namespace LlamaCpp.Net
{
    internal sealed class ModelConstraints
    {
        private readonly SafeLLamaContextHandle _contextHandle;
        private readonly int _nlToken;
        private readonly int _contextSize;


        public ModelConstraints(SafeLLamaContextHandle contextHandle, int nlToken, int contextSize)
        {
            _contextHandle = contextHandle;
            _nlToken = nlToken;
            _contextSize = contextSize;
        }



        public void ApplyConstraints(TokenDataArray candidatesP, IEnumerable<int> lastTokens,
            Span<float> logits,
            int repeatLastTokensCount = 64, float repeatPenalty = 1.1f, float alphaFrequency = .0f,
            float alphaPresence = .0f,
            bool penalizeNl = true
            )
        {
            var nlLogit = logits[_nlToken];

            var lt = lastTokens.ToList();
            var lastTokensCount = lt.Count;
            var lastNRepeat = Math.Min(Math.Min(lastTokensCount, repeatLastTokensCount), _contextSize);


            SampleRepetitionPenalty(_contextHandle, candidatesP,
                lt.Skip(lastTokensCount - lastNRepeat).ToArray(),
                (ulong)lastNRepeat, repeatPenalty);
            SampleFrequencyAndPresencePenalties(_contextHandle, candidatesP,
                lt.Skip(lastTokensCount - lastNRepeat).ToArray(),
                (ulong)lastNRepeat, alphaFrequency, alphaPresence);


            if (!penalizeNl)
            {
                logits[_nlToken] = nlLogit;
            }


        }

        private static unsafe void SampleRepetitionPenalty(SafeLLamaContextHandle ctx,
            TokenDataArray candidates,
            int[] lastTokens, ulong lastTokensSize, float penalty)
        {
            var handle = candidates.data.Pin();
            var st = new TokenDataArrayNative
            {
                data = new IntPtr(handle.Pointer),
                size = candidates.size,
                sorted = candidates.sorted
            };
            ctx.llama_sample_repetition_penalty(new IntPtr(&st), lastTokens, lastTokensSize, penalty);
        }

        private static unsafe void SampleFrequencyAndPresencePenalties(SafeLLamaContextHandle ctx,
            TokenDataArray candidates, int[] lastTokens, ulong lastTokensSize, float alphaFrequency,
            float alphaPresence)
        {
            var handle = candidates.data.Pin();
            var st = new TokenDataArrayNative
            {
                data = new IntPtr(handle.Pointer),
                size = candidates.size,
                sorted = candidates.sorted
            };
            ctx.llama_sample_frequency_and_presence_penalties(new IntPtr(&st), lastTokens,
                lastTokensSize,
                alphaFrequency, alphaPresence);
        }
    }
}
