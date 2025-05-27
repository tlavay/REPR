using Microsoft.REPR.Handlers;
using Microsoft.Test.RERP.Library.Handlers.NotificationRequests;

namespace Microsoft.Test.RERP.Library.Handlers;

internal sealed class SingletonNotificationHandlerOne : ISingletonNotificationHandler<SingletonNotificationRequest>
{
    public Task Send(SingletonNotificationRequest request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
