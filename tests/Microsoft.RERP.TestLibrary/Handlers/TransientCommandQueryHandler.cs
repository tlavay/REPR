using Microsoft.REPR.Handlers;

namespace Microsoft.Test.RERP.Library;

internal sealed class TransientCommandQueryHandler : ITransientCommandQueryRequestHandler<TransientCommandQueryHandlerCommand, TransientCommandQueryHandlerQuery, TransientCommandQueryHandlerResponse>
{
    public Task<TransientCommandQueryHandlerResponse> Handle(TransientCommandQueryHandlerCommand command, TransientCommandQueryHandlerQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TransientCommandQueryHandlerResponse());
    }
}
