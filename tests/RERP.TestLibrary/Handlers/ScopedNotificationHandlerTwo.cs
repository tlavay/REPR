using REPR.Handlers;
using Test.REPR.Library.Handlers.NotificationRequests;

namespace Test.REPR.Library.Handlers;

internal sealed class ScopedNotificationHandlerTwo : IScopedNotificationHandler<ScopedNotificationRequest>
{
    public Task Send(ScopedNotificationRequest request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
