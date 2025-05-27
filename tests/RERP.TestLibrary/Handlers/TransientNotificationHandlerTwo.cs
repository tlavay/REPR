using REPR.Handlers;
using Test.REPR.Library.Handlers.NotificationRequests;

namespace Test.REPR.Library.Handlers;

internal sealed class TransientNotificationHandlerTwo : ITransientNotificationHandler<TransientNotificationRequest>
{
    public Task Send(TransientNotificationRequest request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
