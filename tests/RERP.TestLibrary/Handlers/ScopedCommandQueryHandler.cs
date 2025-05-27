using REPR.Handlers;

namespace Test.REPR.Library;

internal sealed class ScopedCommandQueryHandler : IScopedCommandQueryRequestHandler<ScopedCommandQueryHandlerCommand, ScopedCommandQueryHandlerQuery, ScopedCommandQueryHandlerResponse>
{
    public Task<ScopedCommandQueryHandlerResponse> Handle(ScopedCommandQueryHandlerCommand request, ScopedCommandQueryHandlerQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ScopedCommandQueryHandlerResponse());
    }
}
