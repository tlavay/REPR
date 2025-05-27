using REPR.Handlers;
using Test.REPR.Library.Handlers.NotificationRequests;

namespace Test.REPR.Library.Handlers;

internal sealed class SingletonNotificationHandlerOne : ISingletonNotificationHandler<SingletonNotificationRequest>
{
    public Task Send(SingletonNotificationRequest request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
