using Microsoft.REPR.Handlers;
using Microsoft.Test.RERP.Library.Handlers.NotificationRequests;

namespace Microsoft.Test.RERP.Library.Handlers;

internal sealed class TransientNotificationHandlerOne : ITransientNotificationHandler<TransientNotificationRequest>
{
    public Task Send(TransientNotificationRequest request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
