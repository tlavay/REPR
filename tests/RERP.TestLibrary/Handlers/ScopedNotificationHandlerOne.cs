using REPR.Handlers;
using Test.REPR.Library.Handlers.NotificationRequests;

namespace Test.REPR.Library.Handlers;

internal sealed class ScopedNotificationHandlerOne : IScopedNotificationHandler<ScopedNotificationRequest>
{
    public Task Send(ScopedNotificationRequest request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
