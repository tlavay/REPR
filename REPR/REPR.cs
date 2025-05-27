using Microsoft.Extensions.DependencyInjection;
using REPR.Handlers;

namespace REPR;

internal sealed class REPR : IREPR
{
    private readonly IServiceProvider _serviceProvider;
    public REPR(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<IEnumerable<TResponse>> ExecuteJob<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
    {
        var jobs = _serviceProvider.GetServices<IJobHandler<TRequest, TResponse>>();
        if (jobs == null || !jobs.Any())
        {
            throw new InvalidOperationException($"No IJobHandlers are registered for request type '{typeof(TRequest).FullName}' and response type '{typeof(TResponse).FullName}'. \r\n" +
                $"Make sure you us AddREPR() in startup and the handler is in the executable or in app domain. See README.md");
        }

        var jobTasks = jobs.Select(job => job.Execute(request, cancellationToken));
        return await Task.WhenAll(jobTasks);
    }

    public async Task Send<TRequest>(TRequest request, CancellationToken cancellationToken)
    {
        var subscribers = _serviceProvider.GetServices<INotificationHandler<TRequest>>();
        if (subscribers == null || !subscribers.Any())
        {
            throw new InvalidOperationException($"No INotificationHandler are registered for request type '{typeof(TRequest).FullName}'. \r\n" +
                $"Make sure you us AddREPR() in startup and the handler is in the executable or in app domain. See README.md");
        }

        var subscriberTasks = subscribers.Select(job => job.Send(request, cancellationToken));
        await Task.WhenAll(subscriberTasks);
    }

    public async Task<TResponse> Handle<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
    {
        var handler = _serviceProvider.GetService<IRequestHandler<TRequest, TResponse>>();
        if (handler == null)
        {
            throw new InvalidOperationException($"No IRequestHandler are registered for request type '{typeof(TRequest).FullName}' and response type '{typeof(TResponse).FullName}'. \r\n" +
                $"Make sure you us AddREPR() in startup and the handler is in the executable or in app domain. See README.md");
        }

        return await handler.Handle(request, cancellationToken);
    }

    public async Task<TResponse> Handle<TCommand, TQuery, TResponse>(TCommand command, TQuery query, CancellationToken cancellationToken)
    {
        var handler = _serviceProvider.GetService<ICommandQueryRequestHandler<TCommand, TQuery, TResponse>>();
        if (handler == null)
        {
            throw new InvalidOperationException($"No IRequestHandler are registered for command type '{typeof(TCommand).FullName}', query type '{typeof(TQuery).FullName}' and response type '{typeof(TResponse).FullName}'. \r\n" +
                $"Make sure you us AddREPR() in startup and the handler is in the executable or in app domain. See README.md");
        }

        return await handler.Handle(command, query, cancellationToken);
    }

    public async Task<IEnumerable<TResponse>> Handle<TResponse>(CancellationToken cancellationToken)
    {
        var handler = _serviceProvider.GetService<IListRequestHandler<TResponse>>();
        if (handler == null)
        {
            throw new InvalidOperationException($"No IListRequestHandlers are registered for response type '{typeof(TResponse).FullName}'. \r\n" +
                $"Make sure you us AddREPR() in startup and the handler is in the executable or in app domain. See README.md");
        }

        return await handler.Handle(cancellationToken);
    }
}
