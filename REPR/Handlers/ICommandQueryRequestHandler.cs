namespace REPR.Handlers;

public interface ICommandQueryRequestHandler<TCommand, TQuery, TResponse>
{
    Task<TResponse> Handle(TCommand command, TQuery query, CancellationToken cancellationToken);
}
