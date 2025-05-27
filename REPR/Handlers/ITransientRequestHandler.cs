namespace REPR.Handlers;

public interface ITransientRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
}
