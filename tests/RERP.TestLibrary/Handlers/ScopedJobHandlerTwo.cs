using REPR.Handlers;
using Test.REPR.Library.Handlers.Jobs;

namespace Test.REPR.Library;

internal sealed class ScopedJobHandlerTwo : IScopedJobHandler<ScopedJobRequest, ScopedJobResponse>
{
    public Task<ScopedJobResponse> Execute(ScopedJobRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ScopedJobResponse());
    }
}
