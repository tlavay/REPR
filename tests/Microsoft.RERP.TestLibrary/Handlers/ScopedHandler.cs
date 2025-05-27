using Microsoft.REPR.Handlers;

namespace Microsoft.Test.RERP.Library;

internal sealed class ScopeHandler : IScopedRequestHandler<ScopedHandlerRequest, ScopedHandlerResponse>
{
    public Task<ScopedHandlerResponse> Handle(ScopedHandlerRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ScopedHandlerResponse());
    }
}
