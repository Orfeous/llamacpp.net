using System;
using System.Text;

namespace LlamaCpp.Net.Native.Abstractions;

/// <summary>
///     This exposes a low -level interface to the Llama C++ library.
///     Really, the only reason this exists is to allow for unit testing.
///     That, and to rename the methods to be more C#-like.
/// </summary>
internal interface ILlamaInstance : IDisposable
{
    int ApplyLoraFromFile(string pathLora, string pathBaseModel, int nThreads);
    int GetKeyValueCacheTokenCount();
    void SetRngSeed(int seed);
    ulong GetStateSize();
    ulong CopyStateData(byte[] dest);
    ulong SetStateData(byte[] src);

    unsafe bool LoadSessionFile(string pathSession, int[] tokensOut, ulong nTokenCapacity,
        ulong* nTokenCountOut);

    bool SaveSessionFile(string pathSession, int[] tokens, ulong nTokenCount);
    int Eval(int[] tokens, int nTokens, int nPast, int nThreads);
    unsafe int EvalWithPointer(int* tokens, int nTokens, int nPast, int nThreads);
    int Tokenize(string text, int[] tokens, int nMaxTokens, bool addBos);
    int GetVocabSize();
    int GetContextSize();
    int EmbedSize();
    unsafe float* GetLogits();
    unsafe float* GetEmbeddings();
    void PrintTimings();
    void ResetTimings();
    int GetVocab(string[] strings, float[] scores, int capacity);

    void SampleRepetitionPenalty(IntPtr candidates, int[] lastTokens, ulong lastTokensSize,
        float penalty);

    void SampleFrequencyAndPresencePenalties(IntPtr candidates, int[] lastTokens,
        ulong lastTokensSize, float alphaFrequency, float alphaPresence);

    void SampleSoftmax(IntPtr candidates);
    void SampleTopK(IntPtr candidates, int k, ulong minKeep);
    void SampleTopP(IntPtr candidates, float p, ulong minKeep);
    void SampleTailFree(IntPtr candidates, float z, ulong minKeep);
    void SampleTypical(IntPtr candidates, float p, ulong minKeep);
    void SampleTemperature(IntPtr candidates, float temp);

    /// <summary>
    ///     Mirostat 1.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="candidates"></param>
    /// <param name="tau">
    ///     The target cross-entropy (or surprise) value you want to achieve for the generated text.
    ///     A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less
    ///     surprising or more predictable text.
    /// </param>
    /// <param name="eta">
    ///     The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled
    ///     word.
    ///     A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in
    ///     slower updates.
    /// </param>
    /// <param name="m">
    ///     The number of tokens considered in the estimation of `s_hat`.
    ///     This is an arbitrary value that is used to calculate `s_hat`, which in turn helps to calculate the value of `k`.
    ///     In the paper, they use `m = 100`, but you can experiment with different values to see how it affects the
    ///     performance of the algorithm.
    /// </param>
    /// <param name="mu">
    ///     Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in
    ///     the algorithm based on the error between the target and observed surprisal.
    /// </param>
    /// <returns></returns>
    unsafe int SampleTokenMirostat(IntPtr candidates, float tau, float eta, int m, float* mu);

    /// <summary>
    ///     Mirostat 2.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="candidates"></param>
    /// <param name="tau">
    ///     The target cross-entropy (or surprise) value you want to achieve for the generated text.
    ///     A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less
    ///     surprising or more predictable text.
    /// </param>
    /// <param name="eta">
    ///     The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled
    ///     word.
    ///     A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in
    ///     slower updates.
    /// </param>
    /// <param name="mu">
    ///     Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in
    ///     the algorithm based on the error between the target and observed surprisal.
    /// </param>
    /// <returns></returns>
    unsafe int SampleTokenMirostatV2(IntPtr candidates, float tau, float eta, float* mu);

    /// <summary>
    ///     Selects the token with the highest probability.
    /// </summary>
    /// <param name="candidates"></param>
    /// <returns></returns>
    int SampleTokenGreedy(IntPtr candidates);

    /// <summary>
    ///     Randomly sample a token from the given candidates based on their probabilities.
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="candidates"></param>
    /// <returns></returns>
    int SampleToken(IntPtr candidates);

    IntPtr TokenToStr(int token);
    string TokenToString(int token, Encoding encoding);
}