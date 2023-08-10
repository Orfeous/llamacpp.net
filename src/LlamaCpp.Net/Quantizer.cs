using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Models;
using LlamaCpp.Net.Native;

namespace LlamaCpp.Net;

/// <summary>
///     .net wrapper for the LlamaCpp quantizer
/// </summary>
public class Quantizer : IQuantizer
{

    /// <summary>
    ///     ctor
    /// </summary>
    public Quantizer()
    {
    }


    /// <summary>
    ///     Quantizes the input model file and saves the quantized model to the specified output file path.
    /// </summary>
    /// <param name="outputModelType">The type of the output model file.</param>
    /// <param name="inputModelFilePath"></param>
    /// <param name="outputFilePath">The output file path where the quantized model will be saved.</param>
    /// <param name="threads">The number of threads to use for quantization.</param>
    /// <exception cref="QuantizeException">Thrown when quantization fails with an error code.</exception>
    public void Quantize(string inputModelFilePath, string outputFilePath, ModelType outputModelType, int threads)
    {
        var ftype = Mappers.ModelTypeMapper.ToFType(outputModelType);
        var quantizeResult = LlamaNative.llama_model_quantize(inputModelFilePath, outputFilePath, ftype, threads);

        if (quantizeResult != 0)
        {
            throw new QuantizeException(quantizeResult);
        }
    }
}