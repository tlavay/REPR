namespace Microsoft.REPR.Handlers;

public interface INotificationHandler<TRequest>
{
    Task Send(TRequest request, CancellationToken cancellationToken);
}
