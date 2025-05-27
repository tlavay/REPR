namespace REPR.Handlers;

public interface IScopedNotificationHandler<TRequest> : INotificationHandler<TRequest>
    where TRequest : notnull
{
}
