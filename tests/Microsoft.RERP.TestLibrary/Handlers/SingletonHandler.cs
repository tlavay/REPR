using Microsoft.REPR.Handlers;

namespace Microsoft.Test.RERP.Library;

internal sealed class SingletonHandler : ISingletonRequestHandler<SingletonHandlerRequest, SingletonHandlerResponse>
{
    public Task<SingletonHandlerResponse> Handle(SingletonHandlerRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SingletonHandlerResponse());
    }
}
