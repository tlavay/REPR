using REPR.Handlers;

namespace Test.REPR.Library;

internal sealed class TransientHandler : ITransientRequestHandler<TransientHandlerRequest, TransientHandlerResponse>
{
    public Task<TransientHandlerResponse> Handle(TransientHandlerRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TransientHandlerResponse());
    }
}
