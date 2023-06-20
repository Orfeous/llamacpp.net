using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;

namespace LlamaCpp.Net.Models;
/// <summary>
///    A class to hold the result of an async inference operation
/// </summary>
internal sealed class AsyncInferenceResult
{
    private readonly Channel<string> _channel;

    ///
    internal AsyncInferenceResult()
    {
        _channel = Channel.CreateUnbounded<string>();
    }


    internal bool Append(string token)
    {
        return _channel.Writer.TryWrite(token);
    }

    internal void Complete()
    {
        _channel.Writer.Complete();
    }

    /// <inheritdoc />
    public IAsyncEnumerable<string> ToAsyncEnumerable(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}