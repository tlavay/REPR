using Microsoft.REPR.Handlers;
using Microsoft.Test.RERP.Library.Handlers.Jobs;

namespace Microsoft.Test.RERP.Library;

internal sealed class TransientJobHandlerOne : ITransientJobHandler<TransientJobRequest, TransientJobResponse>
{
    public Task<TransientJobResponse> Execute(TransientJobRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TransientJobResponse());
    }
}
