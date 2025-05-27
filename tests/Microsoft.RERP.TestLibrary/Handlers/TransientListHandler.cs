using Microsoft.REPR.Handlers;

namespace Microsoft.Test.RERP.Library.Handlers;

internal sealed class TransientListHandler : ITransientListRequestHandler<TransientListHandlerResponse>
{
    public Task<IEnumerable<TransientListHandlerResponse>> Handle(CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<TransientListHandlerResponse>>([]);
    }
}
