# REPR

[GitHub Repo](https://github.com/tlavay/REPR)

**R**est **E**nd**P**oint **R**esponse REPR is a lightweight library that implements the Mediator design pattern in .NET. It helps decouple the sender of a request from its handler, promoting cleaner architecture and separation of concerns. Instead of calling services or logic directly, you send a request (like a command or query), and REPR routes it to the appropriate handler.

REPR optimizes dependency injection life cycles by supporting Transient, Scoped, and Singleton handlers.

## Table of Contents

- [Use](#use)
  - [Initialization](#initialization)
    - [Add to service collection](#add-to-service-collection)
    - [REPR Options](#repr-options)
  - [REPR Handlers](#repr-handlers)
    - [Handlers](#handlers)
    - [IRequestHandler](#irequesthandler)
      - [Example IRequestHandler Handler](#example-irequesthandler-handler)
      - [Example GET Controller](#example-get-controller)
    - [IListHandler](#ilisthandler)
      - [Example List Handler](#example-list-handler)
      - [Example List (GET) Controller](#example-list-get-controller)
    - [TransientCommandQueryHandler](#transientcommandqueryhandler)
      - [Example Command Query Handler](#example-command-query-handler)
      - [Example POST Controller](#example-post-controller)
    - [IJobHandler](#ijobhandler)
      - [Example Job Handler](#example-job-handler)
      - [Example POST Jobs Controller](#example-post-jobs-controller)
    - [INotificationHandler](#inotificationhandler)
      - [Example Notification Handler](#example-notification-handler)
      - [Example POST Notification Controller](#example-post-notification-controller)

## Use

### Initialization

To register REPR handlers, extend desired handler and use AddREPR() to service collection. REPR will find any type with a REPR handler and automatically add that handler to the appropriate life cycle **during startup**. Only performance hit to your application will be **during startup**. By default
REPR only adds types from the executing assembly. To specify an assembly in the app domain but not in the executing assembly i.e. a common library assembly. You must include the assembly name in the REPROptions.FilterAssemblies AND REPROptions.IncludeAppDomainAssemblies to true.

#### Add to service collection

```cs
services.AddREPR();
```

#### REPR Options

```cs
public sealed record REPROptions
{
    public bool IncludeAppDomainAssemblies { get; set; }
    public IEnumerable<string>? FilteredAssemblies { get; set; }
    public bool StrictMode { get; set; }
}
```

- IncludeAppDomainAssemblies: will use AppDomain.CurrentDomain.GetAssemblies() if set to true and Assembly.GetExecutingAssembly() if set to false. **Note:** If you set IncludeAppDomainAssemblies to true and do not include a FilteredAssemblies an exception will be thrown.
- FilteredAssemblies: limits the scope of assemblies during search and startup. This is highly recommended for large applications.
- StrictMode: optional and primarily stylistic. Setting this to true will require the handler to be **internal** and **sealed**. This is a runtime error, so be careful. One day I would like to add an SCA to this repo.

### REPR Handlers

REPR handlers default to transient life cycle if base interface is used. To specify a life cycle extend the appropriate interface.

### Handlers

- IRequestHandler<TRequest, TResponse>
- ICommandQueryRequestHandler<TCommand, TQuery, TResponse>
- IListRequestHandler<TResponse>
- IJobHandler<TRequest, TResponse>
- INotificationHandler<TRequest>

#### IRequestHandler

IRequestHandler will search for a **single** implementation of IRequestHandler<TRequest, TResponse> and execute. REPR will throw an exception if none are found.

- Base (Transient): IRequestHandler<TRequest, TResponse>
- Singleton: ISingletonRequestHandler<TRequest, TResponse>
- Scoped: IScopedRequestHandler<TRequest, TResponse>
- Transient: ITransientRequestHandler<TRequest, TResponse>

##### Example IRequestHandler Handler

```cs
using REPR.Handlers;

namespace Test.REPR.Library;

internal sealed class TransientHandler : ITransientRequestHandler<TransientHandlerRequest, TransientHandlerResponse>
{
    public Task<TransientHandlerResponse> Handle(TransientHandlerRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TransientHandlerResponse());
    }
}
```

##### Example GET Controller

```cs
private readonly IREPR _repr;

public FooController(IREPR repr)
{
    _repr = repr;
}

[HttpGet]
public async Task<IActionResult> Get(TRequest request, CancellationToken cancellationToken)
{
    var response = await _repr.Handle<TRequest, TResponse>(request, cancellationToken);
    return Ok(response);
}
```

#### IListHandler

IListHandler will search for a **single** implementation of IListRequestHandler<TResponse> and execute. REPR will throw an exception if none are found.

- Base (Transient): IListRequestHandler<TResponse>
- Singleton: ISingletonListRequestHandler<TResponse>
- Scoped: IScopedListRequestHandler<TResponse>
- Transient: ITransientListRequestHandler<TResponse>

##### Example List Handler

```cs
using REPR.Handlers;

namespace Test.REPR.Library.Handlers;

internal sealed class TransientListHandler : ITransientListRequestHandler<TransientListHandlerResponse>
{
    public Task<IEnumerable<TransientListHandlerResponse>> Handle(CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<TransientListHandlerResponse>>([]);
    }
}
```

##### Example List (GET) Controller

```cs
private readonly IREPR _repr;

public FooController(IREPR repr)
{
    _repr = repr;
}

[HttpGet]
public async Task<IActionResult> List(CancellationToken cancellationToken)
{
    var response = await _repr.Handle<TResponse>(request, cancellationToken);
    return Ok(response);
}
```

#### ITransientCommandQueryHandler

ITransientCommandQueryHandler will search for a **single** implementation of ICommandQueryRequestHandler<TCommand, TQuery, TResponse> and execute. REPR will throw an exception if none are found.

- Base (Transient): ICommandQueryRequestHandler<TCommand, TQuery, TResponse>
- Singleton: ISingletonCommandQueryRequestHandler<TCommand, TQuery, TResponse>
- Scoped: IScopedCommandQueryRequestHandler<TCommand, TQuery, TResponse>
- Transient: ITransientCommandQueryRequestHandler<TCommand, TQuery, TResponse>

##### Example Command Query Handler

```cs
using REPR.Handlers;

namespace Test.REPR.Library;

internal sealed class TransientCommandQueryHandler : ITransientCommandQueryRequestHandler<TransientCommandQueryHandlerCommand, TransientCommandQueryHandlerQuery, TransientCommandQueryHandlerResponse>
{
    public Task<TransientCommandQueryHandlerResponse> Handle(TransientCommandQueryHandlerCommand command, TransientCommandQueryHandlerQuery query, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TransientCommandQueryHandlerResponse());
    }
}
```

##### Example POST Controller

```cs
private readonly IREPR _repr;

public FooController(IREPR repr)
{
    _repr = repr;
}

[HttpPost]
public async Task<IActionResult> Post(TCommand command, TQuery query, CancellationToken cancellationToken)
{
    var response = await _repr.Handle<TCommand, TQuery, TResponse>(command, query, cancellationToken);
    return Ok(response);
}
```

#### IJobHandler

IJobHandler will search for a **ALL** implementation of IJobHandler<TRequest, TResponse> and execute. REPR will throw an exception if none are found.

- Base (Transient): IJobHandler<TRequest, TResponse>
- Singleton: ISingletonJobHandler<TRequest, TResponse>
- Scoped: IScopedJobHandler<TRequest, TResponse>
- Transient: ITransientJobHandler<TRequest, TResponse>

##### Example Job Handler

```cs
using REPR.Handlers;
using Test.REPR.Library.Handlers.Jobs;

namespace Test.REPR.Library;

internal sealed class ScopedJobHandlerOne : IScopedJobHandler<ScopedJobRequest, ScopedJobResponse>
{
    public Task<ScopedJobResponse> Execute(ScopedJobRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new ScopedJobResponse());
    }
}
```

##### Example POST Jobs Controller

```cs
private readonly IREPR _repr;

public FooController(IREPR repr)
{
    _repr = repr;
}

[HttpPost]
public async Task<IActionResult> Post(TRequest request, CancellationToken cancellationToken)
{
    var response = await _repr.ExecuteJob<TRequest, TResponse>(command, cancellationToken);
    return Ok(response);
}
```

#### INotificationHandler

IJobHandler will search for a **ALL** implementation of INotificationHandler<TRequest> and execute. REPR will throw an exception if none are found.

- Base (Transient): INotificationHandler<TRequest>
- Singleton: ISingletonNotificationHandler<TRequest>
- Scoped: IScopedNotificationHandler<TRequest>
- Transient: IScopedNotificationHandler<TRequest>

##### Example Notification Handler

```cs
using REPR.Handlers;
using Test.REPR.Library.Handlers.NotificationRequests;

namespace Test.REPR.Library.Handlers;

internal sealed class TransientNotificationHandlerOne : ITransientNotificationHandler<TransientNotificationRequest>
{
    public Task Send(TransientNotificationRequest request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}

```

##### Example POST Notification Controller

```cs
private readonly IREPR _repr;

public FooController(IREPR repr)
{
    _repr = repr;
}

[HttpPost]
public async Task<IActionResult> Post(TRequest request, CancellationToken cancellationToken)
{
    await _repr.Send<TRequest>(request, cancellationToken);
    return Ok();
}
```
