using REPR.Handlers;
using Test.REPR.Library.Handlers.Jobs;

namespace Test.REPR.Library;

internal sealed class TransientJobHandlerTwo : ITransientJobHandler<TransientJobRequest, TransientJobResponse>
{
    public Task<TransientJobResponse> Execute(TransientJobRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TransientJobResponse());
    }
}
