using System;
using System.Runtime.InteropServices;

namespace LlamaCpp.Net.Native
{
#pragma warning disable CA2101

    using llama_token = Int32;

    /// <summary>
    /// Wrapper around the native library
    /// 
    /// Primary Source: https://github.com/ggerganov/llama.cpp/blob/master/llama.h
    /// Primary Source: https://github.com/ggerganov/llama.cpp/blob/master/ggml.h
    /// 
    /// Secondary Source: https://github.com/abetlen/llama-cpp-python/blob/main/llama_cpp/llama_cpp.py
    /// Secondary Source: https://github.com/SciSharp/LLamaSharp/blob/master/LLama/Native/NativeApi.cs
    /// Secondary Source: https://github.com/hpretila/llama.net/blob/main/LLaMA.NET/Native/LLaMANativeMethods.cs
    /// </summary>
    internal sealed unsafe class LlamaNative
    {
        /// <summary>
        /// todo
        /// </summary>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName, EntryPoint = "llama_mmap_supported", CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool llama_empty_call();

        /// <summary>
        /// todo
        /// </summary>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern LLamaContextParams llama_context_default_params();

        /// <summary>
        /// todo
        /// </summary>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool llama_mmap_supported();

        /// <summary>
        /// todo
        /// </summary>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool llama_mlock_supported();

        /// <summary>
        /// Various functions for loading a ggml llama model.
        /// Allocate (almost) all memory needed for the model.
        /// Return NULL on failure
        /// </summary>
        /// <param name="path_model"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr llama_init_from_file(string path_model, LLamaContextParams @params);

        /// <summary>
        /// not great API - very likely to change. 
        /// Initialize the llama + ggml backend
        /// Call once at the start of the program
        /// </summary>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern void llama_init_backend();

        /// <summary>
        /// Frees all allocated memory
        /// </summary>
        /// <param name="ctx"></param>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern void llama_free(IntPtr ctx);

        /// <summary>
        /// Apply a LoRA adapter to a loaded model
        /// path_base_model is the path to a higher quality model to use as a base for
        /// the layers modified by the adapter. Can be NULL to use the current loaded model.
        /// The model needs to be reloaded before applying a new adapter, otherwise the adapter
        /// will be applied on top of the previous one
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="path_lora"></param>
        /// <param name="path_base_model"></param>
        /// <param name="n_threads"></param>
        /// <returns>Returns 0 on success</returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern int llama_apply_lora_from_file(SafeLLamaContextHandle ctx, string path_lora, string path_base_model, int n_threads);

        /// <summary>
        /// Returns the number of tokens in the KV cache
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern int llama_get_kv_cache_token_count(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Sets the current rng seed.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="seed"></param>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern void llama_set_rng_seed(SafeLLamaContextHandle ctx, int seed);

        /// <summary>
        /// Returns the maximum size in bytes of the state (rng, logits, embedding
        /// and kv_cache) - will often be smaller after compacting tokens
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong llama_get_state_size(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Copies the state to the specified destination address.
        /// Destination needs to have allocated enough memory.
        /// Returns the number of bytes copied
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong llama_copy_state_data(SafeLLamaContextHandle ctx, byte[] dest);

        /// <summary>
        /// Set the state reading from the specified address
        /// Returns the number of bytes read
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="src"></param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern ulong llama_set_state_data(SafeLLamaContextHandle ctx, byte[] src);

        /// <summary>
        /// Load session file
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="path_session"></param>
        /// <param name="tokens_out"></param>
        /// <param name="n_token_capacity"></param>
        /// <param name="n_token_count_out"></param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool llama_load_session_file(SafeLLamaContextHandle ctx, string path_session, llama_token[] tokens_out, ulong n_token_capacity, ulong* n_token_count_out);

        /// <summary>
        /// Save session file
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="path_session"></param>
        /// <param name="tokens"></param>
        /// <param name="n_token_count"></param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern bool llama_save_session_file(SafeLLamaContextHandle ctx, string path_session, llama_token[] tokens, ulong n_token_count);

        /// <summary>
        /// Run the llama inference to obtain the logits and probabilities for the next token.
        /// tokens + n_tokens is the provided batch of new tokens to process
        /// n_past is the number of tokens to use from previous eval calls
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tokens"></param>
        /// <param name="n_tokens"></param>
        /// <param name="n_past"></param>
        /// <param name="n_threads"></param>
        /// <returns>Returns 0 on success</returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern int llama_eval(SafeLLamaContextHandle ctx, llama_token[] tokens, int n_tokens, int n_past, int n_threads);

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="tokens"></param>
        /// <param name="n_tokens"></param>
        /// <param name="n_past"></param>
        /// <param name="n_threads"></param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName, EntryPoint = "llama_eval",  CallingConvention = CallingConvention.Cdecl)]
        internal static extern int llama_eval_with_pointer(SafeLLamaContextHandle ctx, llama_token* tokens, int n_tokens, int n_past, int n_threads);

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="text"></param>
        /// <param name="tokens"></param>
        /// <param name="n_max_tokens"></param>
        /// <param name="add_bos"></param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName, EntryPoint = "llama_tokenize",  CallingConvention = CallingConvention.Cdecl)]
        internal static extern int llama_tokenize(SafeLLamaContextHandle ctx, string text, llama_token[] tokens, int n_max_tokens, bool add_bos);

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern int llama_n_vocab(SafeLLamaContextHandle ctx);

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern int llama_n_ctx(SafeLLamaContextHandle ctx);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern int llama_n_embd(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Token logits obtained from the last call to llama_eval()
        /// The logits for the last token are stored in the last row
        /// Can be mutated in order to change the probabilities of the next token
        /// Rows: n_tokens
        /// Cols: n_vocab
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern float* llama_get_logits(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Get the embeddings for the input
        /// shape: [n_embd] (1-dimensional)
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern float* llama_get_embeddings(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Token Id -> String. Uses the vocabulary in the provided context
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="token"></param>
        /// <returns>Pointer to a string.</returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr llama_token_to_str(SafeLLamaContextHandle ctx, llama_token token);

        /// <summary>
        /// todo
        /// </summary>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern llama_token llama_token_bos();

        /// <summary>
        /// todo
        /// </summary>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern llama_token llama_token_eos();

        /// <summary>
        /// todo
        /// </summary>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern llama_token llama_token_nl();

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="ctx"></param>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern void llama_print_timings(SafeLLamaContextHandle ctx);

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="ctx"></param>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern void llama_reset_timings(SafeLLamaContextHandle ctx);

        /// <summary>
        /// Print system information
        /// </summary>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr llama_print_system_info();


        /// <summary>
        /// LLAMA_API int64_t llama_time_us();
        /// </summary>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern int llama_time_us();

        /// <summary>
        /// Get the vocabulary as output parameters.
        /// Returns number of results.
        /// 
        /// LLAMA_API int llama_get_vocab(const struct llama_context * ctx,const char * * strings, float * scores, int capacity);
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="strings"></param>
        /// <param name="scores"></param>
        /// <param name="capacity"></param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern int llama_get_vocab(SafeLLamaContextHandle ctx, string[] strings, float[] scores, int capacity);

        /// <summary>
        /// Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to TokenDataArray</param>
        /// <param name="last_tokens"></param>
        /// <param name="last_tokens_size"></param>
        /// <param name="penalty"></param>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern void llama_sample_repetition_penalty(SafeLLamaContextHandle ctx, IntPtr candidates, llama_token[] last_tokens, ulong last_tokens_size, float penalty);

        /// <summary>
        /// Frequency and presence penalties described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to TokenDataArray</param>
        /// <param name="last_tokens"></param>
        /// <param name="last_tokens_size"></param>
        /// <param name="alpha_frequency"></param>
        /// <param name="alpha_presence"></param>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern void llama_sample_frequency_and_presence_penalties(SafeLLamaContextHandle ctx, IntPtr candidates, llama_token[] last_tokens, ulong last_tokens_size, float alpha_frequency, float alpha_presence);

        /// <summary>
        /// Sorts candidate tokens by their logits in descending order and calculate probabilities based on logits.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to TokenDataArray</param>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern void llama_sample_softmax(SafeLLamaContextHandle ctx, IntPtr candidates);

        /// <summary>
        /// Top-K sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to TokenDataArray</param>
        /// <param name="k"></param>
        /// <param name="min_keep"></param>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern void llama_sample_top_k(SafeLLamaContextHandle ctx, IntPtr candidates, int k, ulong min_keep);

        /// <summary>
        /// Nucleus sampling described in academic paper "The Curious Case of Neural Text Degeneration" https://arxiv.org/abs/1904.09751
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to TokenDataArray</param>
        /// <param name="p"></param>
        /// <param name="min_keep"></param>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern void llama_sample_top_p(SafeLLamaContextHandle ctx, IntPtr candidates, float p, ulong min_keep);

        /// <summary>
        /// Tail Free Sampling described in https://www.trentonbricken.com/Tail-Free-Sampling/.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to TokenDataArray</param>
        /// <param name="z"></param>
        /// <param name="min_keep"></param>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern void llama_sample_tail_free(SafeLLamaContextHandle ctx, IntPtr candidates, float z, ulong min_keep);

        /// <summary>
        /// Locally Typical Sampling implementation described in the paper https://arxiv.org/abs/2202.00666.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to TokenDataArray</param>
        /// <param name="p"></param>
        /// <param name="min_keep"></param>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern void llama_sample_typical(SafeLLamaContextHandle ctx, IntPtr candidates, float p, ulong min_keep);

        /// <summary>
        /// todo
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates"></param>
        /// <param name="temp"></param>
        [DllImport(LibraryLoader.NativeLibraryName, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void llama_sample_temperature(SafeLLamaContextHandle ctx, IntPtr candidates, float temp);

        /// <summary>
        /// Mirostat 1.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">A vector of `llama_token_data` containing the candidate tokens, their probabilities (p), and log-odds (logit) for the current position in the generated text.</param>
        /// <param name="tau">The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.</param>
        /// <param name="eta">The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.</param>
        /// <param name="m">The number of tokens considered in the estimation of `s_hat`. This is an arbitrary value that is used to calculate `s_hat`, which in turn helps to calculate the value of `k`. In the paper, they use `m = 100`, but you can experiment with different values to see how it affects the performance of the algorithm.</param>
        /// <param name="mu">Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in the algorithm based on the error between the target and observed surprisal.</param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern llama_token llama_sample_token_mirostat(SafeLLamaContextHandle ctx, IntPtr candidates, float tau, float eta, int m, float* mu);

        /// <summary>
        /// Mirostat 2.0 algorithm described in the paper https://arxiv.org/abs/2007.14966. Uses tokens instead of words.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">A vector of `llama_token_data` containing the candidate tokens, their probabilities (p), and log-odds (logit) for the current position in the generated text.</param>
        /// <param name="tau">The target cross-entropy (or surprise) value you want to achieve for the generated text. A higher value corresponds to more surprising or less predictable text, while a lower value corresponds to less surprising or more predictable text.</param>
        /// <param name="eta">The learning rate used to update `mu` based on the error between the target and observed surprisal of the sampled word. A larger learning rate will cause `mu` to be updated more quickly, while a smaller learning rate will result in slower updates.</param>
        /// <param name="mu">Maximum cross-entropy. This value is initialized to be twice the target cross-entropy (`2 * tau`) and is updated in the algorithm based on the error between the target and observed surprisal.</param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern llama_token llama_sample_token_mirostat_v2(SafeLLamaContextHandle ctx, IntPtr candidates, float tau, float eta, float* mu);

        /// <summary>
        /// Selects the token with the highest probability.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to TokenDataArray</param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern llama_token llama_sample_token_greedy(SafeLLamaContextHandle ctx, IntPtr candidates);

        /// <summary>
        /// Randomly selects a token from the candidates based on their probabilities.
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="candidates">Pointer to TokenDataArray</param>
        /// <returns></returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern llama_token llama_sample_token(SafeLLamaContextHandle ctx, IntPtr candidates);

        /// <summary>
        /// Returns 0 on success
        /// </summary>
        /// <param name="fname_inp"></param>
        /// <param name="fname_out"></param>
        /// <param name="ftype"></param>
        /// <param name="nthread">how many threads to use. If &lt;=0, will use std::thread::hardware_concurrency(), else the number given</param>
        /// <remarks>not great API - very likely to change</remarks>
        /// <returns>Returns 0 on success</returns>
        [DllImport(LibraryLoader.NativeLibraryName,  CallingConvention = CallingConvention.Cdecl)]
        internal static extern int llama_model_quantize(string fname_inp, string fname_out, LLamaFtype ftype, int nthread);
    }
}
#pragma warning restore CA2101
