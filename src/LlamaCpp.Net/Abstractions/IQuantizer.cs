namespace LlamaCpp.Net.Abstractions;

/// <summary>
/// </summary>
public interface IQuantizer
{
    /// <summary>
    ///     Quantizes the input model file and saves the quantized model to the specified output file path.
    /// </summary>
    /// <param name="inputModelFilePath"></param>
    /// <param name="outputModelType">The type of the output model file.</param>
    /// <param name="outputFilePath">The output file path where the quantized model will be saved.</param>
    /// <param name="threads">The number of threads to use for quantization.</param>
    /// <exception cref="QuantizeException">Thrown when quantization fails with an error code.</exception>
    void Quantize(string inputModelFilePath, string outputFilePath, ModelType outputModelType, int threads);
}