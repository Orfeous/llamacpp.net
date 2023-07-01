﻿using LlamaCpp.Net.Native.Abstractions;
using LlamaCpp.Net.Native.Models;
using System;

namespace LlamaCpp.Net.Extensions
{
    /// <summary>
    /// Extensions for the native library
    /// </summary>
    internal static class LlamaNativeExtensions
    {
        /// <summary>
        /// Randomly sample a token from the given candidates based on their probabilities.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates"></param>
        /// <returns></returns>
        internal static unsafe int SampleToken(this ILlamaInstance ctx, TokenDataArray candidates)
        {
            var handle = candidates.data.Pin();
            var st = new TokenDataArrayNative
            {
                data = new IntPtr(handle.Pointer),
                size = candidates.size,
                sorted = candidates.sorted
            };
            return ctx.SampleToken(new IntPtr(&st));
        }

        /// <summary>
        /// Mirostat 1.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates"></param>
        /// <param name="tau">
        ///The target cross-entropy (or surprise) value you want to achieve for the generated text.
        /// A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.
        /// </param>
        /// <param name="eta">
        /// The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word.
        /// A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.
        /// </param>
        /// <param name="m">
        /// The number of tokens considered in the estimation of `s_hat`.
        /// This is an arbitrary value that is used to calculate `s_hat`, which in turn helps to calculate the value of `k`.
        /// In the paper, they use `m = 100`, but you can experiment with different values to see how it affects the performance of the algorithm.
        /// </param>
        /// <param name="mu">
        /// Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in the algorithm based on the error between the target and observed surprisal.
        /// </param>
        /// <returns></returns>
        internal static unsafe int SampleMirostat(this ILlamaInstance ctx, TokenDataArray candidates, float tau,
            float eta, int m, float* mu)
        {
            var handle = candidates.data.Pin();
            var st = new TokenDataArrayNative
            {
                data = new IntPtr(handle.Pointer),
                size = candidates.size,
                sorted = candidates.sorted
            };
            return ctx.SampleTokenMirostat(new IntPtr(&st), tau, eta, m, mu);
        }

        ///  <summary>
        ///  Mirostat 2.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.
        ///  </summary>
        ///  <param name="ctx"></param>
        ///  <param name="candidates"></param>
        ///  <param name="tau">
        /// The target cross-entropy (or surprise) value you want to achieve for the generated text.
        /// A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.
        ///  </param>
        ///  <param name="eta">
        ///  The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word.
        /// A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.
        ///  </param>
        ///  <param name="mu">
        ///  Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in the algorithm based on the error between the target and observed surprisal.
        ///  </param>
        ///  <returns></returns>
        internal static unsafe int SampleMirostatV2(this ILlamaInstance ctx, TokenDataArray candidates,
            float tau,
            float eta, float* mu)
        {
            var handle = candidates.data.Pin();
            var st = new TokenDataArrayNative
            {
                data = new IntPtr(handle.Pointer),
                size = candidates.size,
                sorted = candidates.sorted
            };
            return ctx.SampleTokenMirostatV2(new IntPtr(&st), tau, eta, mu);
        }

        /// <summary>
        /// Selects the token with the highest probability.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates"></param>
        /// <returns></returns>
        internal static unsafe int SampleGreedy(this ILlamaInstance ctx, TokenDataArray candidates)
        {
            var handle = candidates.data.Pin();
            var st = new TokenDataArrayNative
            {
                data = new IntPtr(handle.Pointer),
                size = candidates.size,
                sorted = candidates.sorted
            };
            return ctx.SampleTokenGreedy(new IntPtr(&st));
        }
    }
}