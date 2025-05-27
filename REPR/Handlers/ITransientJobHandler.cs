namespace REPR.Handlers;

public interface ITransientJobHandler<TRequest, TResponse> : IJobHandler<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
}
