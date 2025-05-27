namespace REPR.Handlers;

public interface ISingletonListRequestHandler<TResponse> : IListRequestHandler<TResponse>
    where TResponse : notnull
{
}
