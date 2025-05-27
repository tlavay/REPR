using REPR.Handlers;
using Test.REPR.Library.Handlers.Jobs;

namespace Test.REPR.Library;

internal sealed class SingletonJobHandlerTwo : ISingletonJobHandler<SingletonJobRequest, SingletonJobResponse>
{
    public Task<SingletonJobResponse> Execute(SingletonJobRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SingletonJobResponse());
    }
}
