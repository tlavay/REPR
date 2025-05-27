namespace Microsoft.REPR.Handlers;

public interface ITransientCommandQueryRequestHandler<TCommand, TQuery, TResponse> : ICommandQueryRequestHandler<TCommand, TQuery, TResponse>
    where TCommand : notnull
    where TQuery : notnull
    where TResponse : notnull
{
}
