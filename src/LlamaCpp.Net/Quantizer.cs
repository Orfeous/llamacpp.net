using LlamaCpp.Net.Abstractions;
using LlamaCpp.Net.Exceptions;
using LlamaCpp.Net.Models;
using LlamaCpp.Net.Native;

namespace LlamaCpp.Net;

/// <summary>
///     .net wrapper for the LlamaCpp quantizer
/// </summary>
public class Quantizer : IQuantizer
{
    private readonly string _inputModelFilePath;

    /// <summary>
    ///     ctor
    /// </summary>
    /// <param name="inputModelFilePath"></param>
    public Quantizer(string inputModelFilePath)
    {
        _inputModelFilePath = inputModelFilePath;
    }


    /// <summary>
    ///     Quantizes the input model file and saves the quantized model to the specified output file path.
    /// </summary>
    /// <param name="modelType">The type of the output model file.</param>
    /// <param name="outputFilePath">The output file path where the quantized model will be saved.</param>
    /// <param name="threads">The number of threads to use for quantization.</param>
    /// <exception cref="QuantizeException">Thrown when quantization fails with an error code.</exception>
    public void Quantize(ModelType modelType, string outputFilePath, int threads)
    {
        var ftype = Mappers.ModelTypeMapper.ToFType(modelType);
        var quantizeResult = LlamaNative.llama_model_quantize(_inputModelFilePath, outputFilePath, ftype, threads);

        if (quantizeResult != 0)
        {
            throw new QuantizeException(quantizeResult);
        }
    }
}