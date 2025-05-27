using Microsoft.REPR.Handlers;
using Microsoft.Test.RERP.Library.Handlers.Jobs;

namespace Microsoft.Test.RERP.Library;

internal sealed class SingletonJobHandlerTwo : ISingletonJobHandler<SingletonJobRequest, SingletonJobResponse>
{
    public Task<SingletonJobResponse> Execute(SingletonJobRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SingletonJobResponse());
    }
}
