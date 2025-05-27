namespace REPR;

public interface IREPR
{
    public Task<TResponse> Handle<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken);
    public Task<IEnumerable<TResponse>> ExecuteJob<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken);
    public Task<TResponse> Handle<TCommand, TQuery, TResponse>(TCommand command, TQuery query, CancellationToken cancellationToken);
    public Task<IEnumerable<TResponse>> Handle<TResponse>(CancellationToken cancellationToken);
    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken);
}
