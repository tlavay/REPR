namespace REPR.Handlers;

public interface ISingletonJobHandler<TRequest, TResponse> : IJobHandler<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
}
