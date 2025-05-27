namespace REPR.Handlers;

public interface IJobHandler<TRequest, TResponse>
{
    Task<TResponse> Execute(TRequest request, CancellationToken cancellationToken);
}
