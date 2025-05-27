namespace REPR.Handlers;

public interface IScopedCommandQueryRequestHandler<TCommand, TQuery, TResponse> : ICommandQueryRequestHandler<TCommand, TQuery, TResponse>
    where TCommand : notnull
    where TQuery : notnull
    where TResponse : notnull
{
}
