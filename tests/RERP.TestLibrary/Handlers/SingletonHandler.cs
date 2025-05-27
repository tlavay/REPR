using REPR.Handlers;

namespace Test.REPR.Library;

internal sealed class SingletonHandler : ISingletonRequestHandler<SingletonHandlerRequest, SingletonHandlerResponse>
{
    public Task<SingletonHandlerResponse> Handle(SingletonHandlerRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SingletonHandlerResponse());
    }
}
