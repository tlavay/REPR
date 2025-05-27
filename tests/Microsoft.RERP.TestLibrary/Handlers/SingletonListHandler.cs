using Microsoft.REPR.Handlers;

namespace Microsoft.Test.RERP.Library.Handlers;

internal sealed class SingletonListHandler : ISingletonListRequestHandler<SingletonListHandlerResponse>
{
    public Task<IEnumerable<SingletonListHandlerResponse>> Handle(CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<SingletonListHandlerResponse>>([]);
    }
}