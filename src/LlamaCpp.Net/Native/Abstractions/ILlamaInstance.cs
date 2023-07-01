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
    unsafe int SampleTokenMirostat(IntPtr candidates, float tau, float eta, int m, float* mu);
    unsafe int SampleTokenMirostatV2(IntPtr candidates, float tau, float eta, float* mu);
    int SampleTokenGreedy(IntPtr candidates);
    int SampleToken(IntPtr candidates);
    IntPtr TokenToStr(int token);
    string TokenToString(int token, Encoding encoding);
}