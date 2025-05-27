using Microsoft.REPR.Handlers;

namespace Microsoft.Test.RERP.Library;

internal sealed class ScopedCommandQueryHandler : IScopedCommandQueryRequestHandler<ScopedCommandQueryHandlerCommand, ScopedCommandQueryHandlerQuery, ScopedCommandQueryHandlerResponse>
{
    public Task<ScopedCommandQueryHandlerResponse> Handle(ScopedCommandQueryHandlerCommand request, ScopedCommandQueryHandlerQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ScopedCommandQueryHandlerResponse());
    }
}
