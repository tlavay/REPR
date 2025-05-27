using REPR.Handlers;

namespace Test.REPR.Library;

internal sealed class TransientCommandQueryHandler : ITransientCommandQueryRequestHandler<TransientCommandQueryHandlerCommand, TransientCommandQueryHandlerQuery, TransientCommandQueryHandlerResponse>
{
    public Task<TransientCommandQueryHandlerResponse> Handle(TransientCommandQueryHandlerCommand command, TransientCommandQueryHandlerQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TransientCommandQueryHandlerResponse());
    }
}
