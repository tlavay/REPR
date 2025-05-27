namespace REPR.Handlers;

public interface ISingletonCommandQueryRequestHandler<TCommand, TQuery, TResponse> : ICommandQueryRequestHandler<TCommand, TQuery, TResponse>
    where TCommand : notnull
    where TQuery : notnull
    where TResponse : notnull
{
}
