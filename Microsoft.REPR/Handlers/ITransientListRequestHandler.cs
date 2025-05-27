namespace Microsoft.REPR.Handlers;

public interface ITransientListRequestHandler<TResponse> : IListRequestHandler<TResponse>
    where TResponse : notnull
{
}
