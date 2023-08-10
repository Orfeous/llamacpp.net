using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using LlamaCpp.Net.Exceptions;
using LlamaCpp.Net.Native.Abstractions;

namespace LlamaCpp.Net.Native.API;

internal unsafe class LlamaInstance : ILlamaInstance
{
    private readonly SafeLLamaContextHandle _contextHandle;

    public LlamaInstance(string modelPath, LLamaContextParams contextParams)
    {
        IntPtr context;
        try
        {
            context = LlamaNative.llama_init_from_file(modelPath, contextParams);
        }
        catch (SEHException e)
        {
            throw new ModelFailedInitializationException(modelPath, e);
        }

        if (context == IntPtr.Zero)
        {
            throw new ModelFailedInitializationException(modelPath);
        }

        _contextHandle = new SafeLLamaContextHandle(context);
    }

    public void Dispose()
    {
        _contextHandle.Dispose();
    }

    public int ApplyLoraFromFile(string pathLora, string pathBaseModel, int nThreads)
    {
        return LlamaNative.llama_apply_lora_from_file(_contextHandle, pathLora, pathBaseModel, nThreads);
    }

    public int GetKeyValueCacheTokenCount()
    {
        return LlamaNative.llama_get_kv_cache_token_count(_contextHandle);
    }

    public void SetRngSeed(int seed)
    {
        LlamaNative.llama_set_rng_seed(_contextHandle, seed);
    }

    public ulong GetStateSize()
    {
        return LlamaNative.llama_get_state_size(_contextHandle);
    }

    public ulong CopyStateData(byte[] dest)
    {
        return LlamaNative.llama_copy_state_data(_contextHandle, dest);
    }

    public ulong SetStateData(byte[] src)
    {
        return LlamaNative.llama_set_state_data(_contextHandle, src);
    }

    public bool LoadSessionFile(string pathSession, int[] tokensOut, ulong nTokenCapacity,
        ulong* nTokenCountOut)
    {
        return LlamaNative.llama_load_session_file(_contextHandle, pathSession, tokensOut, nTokenCapacity,
            nTokenCountOut);
    }

    public bool SaveSessionFile(string pathSession, int[] tokens, ulong nTokenCount)
    {
        return LlamaNative.llama_save_session_file(_contextHandle, pathSession, tokens, nTokenCount);
    }

    public int Eval(int[] tokens, int nTokens, int nPast, int nThreads)
    {
        return LlamaNative.llama_eval(_contextHandle, tokens, nTokens, nPast, nThreads);
    }

    public int EvalWithPointer(int* tokens, int nTokens, int nPast, int nThreads)
    {
        return LlamaNative.llama_eval_with_pointer(_contextHandle, tokens, nTokens, nPast, nThreads);
    }

    public int Tokenize(string text, int[] tokens, int nMaxTokens, bool addBos)
    {
        return LlamaNative.llama_tokenize(_contextHandle, text, tokens, nMaxTokens, addBos);
    }

    public int GetVocabSize()
    {
        return LlamaNative.llama_n_vocab(_contextHandle);
    }

    public int GetContextSize()
    {
        return LlamaNative.llama_n_ctx(_contextHandle);
    }

    public int EmbedSize()
    {
        return LlamaNative.llama_n_embd(_contextHandle);
    }

    public float* GetLogits()
    {
        return LlamaNative.llama_get_logits(_contextHandle);
    }

    public float* GetEmbeddings()
    {
        return LlamaNative.llama_get_embeddings(_contextHandle);
    }


    public void PrintTimings()
    {
        LlamaNative.llama_print_timings(_contextHandle);
    }

    public void ResetTimings()
    {
        LlamaNative.llama_reset_timings(_contextHandle);
    }

    public int GetVocab(string[] strings, float[] scores, int capacity)
    {
        return LlamaNative.llama_get_vocab(_contextHandle, strings, scores, capacity);
    }

    public void SampleRepetitionPenalty(IntPtr candidates, int[] lastTokens, ulong lastTokensSize,
        float penalty)
    {
        LlamaNative.llama_sample_repetition_penalty(_contextHandle, candidates, lastTokens, lastTokensSize, penalty);
    }

    public void SampleFrequencyAndPresencePenalties(IntPtr candidates, int[] lastTokens,
        ulong lastTokensSize, float alphaFrequency, float alphaPresence)
    {
        LlamaNative.llama_sample_frequency_and_presence_penalties(_contextHandle, candidates, lastTokens,
            lastTokensSize,
            alphaFrequency, alphaPresence);
    }


    public void SampleSoftmax(IntPtr candidates)
    {
        LlamaNative.llama_sample_softmax(_contextHandle, candidates);
    }

    public void SampleTopK(IntPtr candidates, int k, ulong minKeep)
    {
        LlamaNative.llama_sample_top_k(_contextHandle, candidates, k, minKeep);
    }

    public void SampleTopP(IntPtr candidates, float p, ulong minKeep)
    {
        LlamaNative.llama_sample_top_p(_contextHandle, candidates, p, minKeep);
    }

    public void SampleTailFree(IntPtr candidates, float z, ulong minKeep)
    {
        LlamaNative.llama_sample_tail_free(_contextHandle, candidates, z, minKeep);
    }

    public void SampleTypical(IntPtr candidates, float p, ulong minKeep)
    {
        LlamaNative.llama_sample_typical(_contextHandle, candidates, p, minKeep);
    }

    public void SampleTemperature(IntPtr candidates, float temp)
    {
        LlamaNative.llama_sample_temperature(_contextHandle, candidates, temp);
    }

    public int SampleTokenMirostat(IntPtr candidates, float tau, float eta, int m, float* mu)
    {
        return LlamaNative.llama_sample_token_mirostat(_contextHandle, candidates, tau, eta, m, mu);
    }

    public int SampleTokenMirostatV2(IntPtr candidates, float tau, float eta, float* mu)
    {
        return LlamaNative.llama_sample_token_mirostat_v2(_contextHandle, candidates, tau, eta, mu);
    }

    public int SampleTokenGreedy(IntPtr candidates)
    {
        return LlamaNative.llama_sample_token_greedy(_contextHandle, candidates);
    }

    public int SampleToken(IntPtr candidates)
    {
        return LlamaNative.llama_sample_token(_contextHandle, candidates);
    }

    public IntPtr TokenToStr(int token)
    {
        return LlamaNative.llama_token_to_str(_contextHandle, token);
    }

    public string TokenToString(int token, Encoding encoding)
    {
        var ptr = TokenToStr(token);
        var s = PtrToString(ptr, encoding);

        return s;
    }
    /// <summary>
    ///    Converts a stringPointer to a string
    /// </summary>
    /// <param name="stringPointer"></param>
    /// <param name="encoding"></param>
    /// <returns></returns>
    private static unsafe string PtrToString(IntPtr stringPointer, Encoding encoding)
    {
#if NET6_0_OR_GREATER
            if(encoding == Encoding.UTF8)
            {
                return Marshal.PtrToStringUTF8(stringPointer);
            }
            else if(encoding == Encoding.Unicode)
            {
                return Marshal.PtrToStringUni(stringPointer);
            }
            else
            {
                return Marshal.PtrToStringAuto(stringPointer);
            }
#else
        var tp = (byte*)stringPointer.ToPointer();
        List<byte> bytes = new List<byte>();
        while (true)
        {
            var c = *tp++;
            if (c == '\0')
            {
                break;
            }

            bytes.Add(c);
        }

        return encoding.GetString(bytes.ToArray());
#endif
    }
}