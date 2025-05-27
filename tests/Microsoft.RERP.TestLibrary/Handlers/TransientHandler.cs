using Microsoft.REPR.Handlers;

namespace Microsoft.Test.RERP.Library;

internal sealed class TransientHandler : ITransientRequestHandler<TransientHandlerRequest, TransientHandlerResponse>
{
    public Task<TransientHandlerResponse> Handle(TransientHandlerRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TransientHandlerResponse());
    }
}
