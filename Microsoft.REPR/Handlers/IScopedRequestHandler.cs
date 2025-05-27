namespace Microsoft.REPR.Handlers;

public interface IScopedRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
}
