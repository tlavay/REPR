using REPR.Handlers;

namespace Test.REPR.Library.Handlers;

internal sealed class SingletonListHandler : ISingletonListRequestHandler<SingletonListHandlerResponse>
{
    public Task<IEnumerable<SingletonListHandlerResponse>> Handle(CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<SingletonListHandlerResponse>>([]);
    }
}