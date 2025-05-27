namespace REPR.Handlers;

public interface IScopedJobHandler<TRequest, TResponse> : IJobHandler<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
}
