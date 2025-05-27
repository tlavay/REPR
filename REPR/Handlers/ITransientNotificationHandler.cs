namespace REPR.Handlers;

public interface ITransientNotificationHandler<TRequest> : INotificationHandler<TRequest>
    where TRequest : notnull
{
}
