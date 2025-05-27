using REPR.Handlers;

namespace Test.REPR.Library;

internal sealed class SingletonCommandQueryHandler : ISingletonCommandQueryRequestHandler<SingletonCommandQueryHandlerCommand, SingletonCommandQueryHandlerQuery, SingletonCommandQueryHandlerResponse>
{
    public Task<SingletonCommandQueryHandlerResponse> Handle(SingletonCommandQueryHandlerCommand command, SingletonCommandQueryHandlerQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SingletonCommandQueryHandlerResponse());
    }
}
