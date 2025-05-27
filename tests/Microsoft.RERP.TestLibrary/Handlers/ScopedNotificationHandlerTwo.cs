using Microsoft.REPR.Handlers;
using Microsoft.Test.RERP.Library.Handlers.NotificationRequests;

namespace Microsoft.Test.RERP.Library.Handlers;

internal sealed class ScopedNotificationHandlerTwo : IScopedNotificationHandler<ScopedNotificationRequest>
{
    public Task Send(ScopedNotificationRequest request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
