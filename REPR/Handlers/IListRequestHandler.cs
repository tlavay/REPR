namespace REPR.Handlers;

public interface IListRequestHandler<TResponse>
{
    Task<IEnumerable<TResponse>> Handle(CancellationToken cancellationToken);
}
