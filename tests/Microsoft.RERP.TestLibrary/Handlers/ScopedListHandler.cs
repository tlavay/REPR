using Microsoft.REPR.Handlers;

namespace Microsoft.Test.RERP.Library.Handlers;

internal sealed class ScopedListHandler : IScopedListRequestHandler<ScopedListHandlerResponse>
{
    public Task<IEnumerable<ScopedListHandlerResponse>> Handle(CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<ScopedListHandlerResponse>>([]);
    }
}
