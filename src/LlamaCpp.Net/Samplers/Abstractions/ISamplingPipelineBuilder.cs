using LlamaCpp.Net.Configuration;
using LlamaCpp.Net.Native.Abstractions;
using LlamaCpp.Net.Samplers.Pipelines;

namespace LlamaCpp.Net.Samplers.Abstractions;

/// <summary>
/// </summary>
public interface ISamplingPipelineBuilder
{
    /// <summary>
    ///     Temperature controls the randomness of the model's output.
    ///     It affects the probability distribution of the model's output tokens.
    ///     A higher temperature (e.g., 1.5) makes the output more random and creative,
    ///     while a lower temperature (e.g., 0.5) makes the output more focused, deterministic, and conservative.
    ///     The default value is 0.8, which provides a balance between randomness and determinism.
    ///     At the extreme, a temperature of 0 will always pick the most likely next token, leading to identical outputs in
    ///     each run.
    /// </summary>
    ISamplingPipelineBuilder AddTemperatureSampler(float temperature);

    /// <summary>
    ///     Sorts candidate tokens by their logits in descending order and calculate probabilities based on logits.
    /// </summary>
    ISamplingPipelineBuilder AddSoftMaxSampler();

    /// <summary>
    ///     Tail Free Sampling described in https://www.trentonbricken.com/Tail-Free-Sampling/.
    ///     TFS  is a text generation technique that aims to reduce the impact of less likely tokens, which may be less
    ///     relevant, less coherent, or nonsensical, on the output.
    ///     The method adjusts the logits (token probabilities) by raising them to the power of the parameter z.
    ///     A higher value of z (e.g., 2.0) will further suppress less likely tokens from the tail of the distribution, while a
    ///     value of 1.0 disables the effect of TFS.
    ///     By setting the parameter z, you can control how much the probabilities of less likely tokens are reduced
    /// </summary>
    ISamplingPipelineBuilder AddTailFreeSampler(int tailFreeZ, ulong minKeep);


    /// <summary>
    ///     Nucleus sampling described in academic paper "The Curious Case of Neural Text Degeneration"
    ///     https://arxiv.org/abs/1904.09751
    ///     Top-p sampling, also known as nucleus sampling, is a text generation method that selects the next token
    ///     from a subset of tokens that together have a cumulative probability of at least p.
    ///     This method provides a balance between diversity and quality by considering both the probabilities of tokens and
    ///     the number of tokens to sample from.
    ///     A higher value for top-p (e.g., 0.95) will lead to more diverse text, while a lower value (e.g., 0.5) will generate
    ///     more focused and conservative text.
    ///     The default value for llama.cpp is 0.9.
    /// </summary>
    ISamplingPipelineBuilder AddTopPSampler(float topP, ulong minKeep);

    /// <summary>
    ///     Top-K sampling described in academic paper "The Curious Case of Neural Text Degeneration"
    ///     https://arxiv.org/abs/1904.09751
    ///     Top-k sampling is a text generation method that selects the next token only from the top k most likely tokens
    ///     predicted by the model.
    ///     It helps reduce the risk of generating low-probability or nonsensical tokens, but it may also limit the diversity
    ///     of the output.
    ///     A higher value for top-k (e.g., 100) will consider more tokens and lead to more diverse text, while a lower value
    ///     (e.g., 10) will focus on the most probable tokens and generate more conservative text.
    ///     A reasonable value, and the default for Llama.cpp is 40.
    /// </summary>
    ISamplingPipelineBuilder AddTopKSampler(int topK, ulong minKeep);


    /// <summary>
    ///     Locally Typical Sampling implementation described in the paper https://arxiv.org/abs/2202.00666.
    ///     Locally typical sampling promotes the generation of contextually coherent and diverse text
    ///     by sampling tokens that are typical or expected based on the surrounding context.
    ///     By setting the parameter p between 0 and 1, you can control the balance between producing text that is locally
    ///     coherent and diverse.
    ///     A value closer to 1 will promote more contextually coherent tokens,
    ///     while a value closer to 0 will promote more diverse tokens. A value equal to 1 disables locally typical sampling.
    /// </summary>
    ISamplingPipelineBuilder AddTypicalSampler(int localTypicalK, ulong minKeep);


    /// <summary>
    ///     Apply repetition penalty to the candidates
    ///     Repetition penalty described in CTRL academic paper https://arxiv.org/abs/1909.05858, with negative logit fix.
    /// </summary>
    /// <param name="penalty">
    ///     The RepetitionPenalty parameter helps prevent the model from generating repetitive or monotonous text.
    ///     A higher value (e.g., 1.5) will penalize repetitions more strongly,
    ///     while a lower value (e.g., 0.9) will be more lenient.
    /// </param>
    /// <returns></returns>
    ISamplingPipelineBuilder AddRepetitionPenaltySampler(float penalty);


    /// <summary>
    ///     Apply frequency and presence penalties to the candidates
    ///     described in OpenAI API https://platform.openai.com/docs/api-reference/parameter-details.
    /// </summary>
    /// <param name="alphaFrequency">
    ///     The frequency of the alpha parameter. Alpha controls the degree of randomness in the
    ///     model's output. A reasonable value for this parameter is between 0.1 and 1, with 1 being the most random.
    /// </param>
    /// ">
    /// <param name="alphaPresence">
    ///     The presence of the alpha parameter. Alpha controls the degree of randomness in the model's
    ///     output. A reasonable value for this parameter is between 0.1 and 1, with 1 being the most random.
    /// </param>
    /// ">
    ISamplingPipelineBuilder AddFrequencyAndPresencePenaltySampler(float alphaFrequency, float alphaPresence);

    internal SamplingPipeline Build(ILlamaInstance instance);
    ISamplingPipelineBuilder SetEndSampler(SamplingMethod endSampler);
}