namespace Microsoft.REPR.Handlers;

public interface IScopedListRequestHandler<TResponse> : IListRequestHandler<TResponse>
    where TResponse : notnull
{
}
