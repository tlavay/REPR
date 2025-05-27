using Microsoft.REPR.Handlers;
using Microsoft.Test.RERP.Library.Handlers.Jobs;

namespace Microsoft.Test.RERP.Library;

internal sealed class ScopedJobHandlerTwo : IScopedJobHandler<ScopedJobRequest, ScopedJobResponse>
{
    public Task<ScopedJobResponse> Execute(ScopedJobRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ScopedJobResponse());
    }
}
