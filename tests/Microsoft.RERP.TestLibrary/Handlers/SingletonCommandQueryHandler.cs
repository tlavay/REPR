using Microsoft.REPR.Handlers;

namespace Microsoft.Test.RERP.Library;

internal sealed class SingletonCommandQueryHandler : ISingletonCommandQueryRequestHandler<SingletonCommandQueryHandlerCommand, SingletonCommandQueryHandlerQuery, SingletonCommandQueryHandlerResponse>
{
    public Task<SingletonCommandQueryHandlerResponse> Handle(SingletonCommandQueryHandlerCommand command, SingletonCommandQueryHandlerQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new SingletonCommandQueryHandlerResponse());
    }
}
