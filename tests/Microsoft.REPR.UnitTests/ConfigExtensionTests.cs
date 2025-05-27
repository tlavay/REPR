using Microsoft.Extensions.DependencyInjection;
using Microsoft.REPR.Constants;
using Microsoft.REPR.Exceptions;
using Microsoft.REPR.Handlers;
using Microsoft.REPR.Models;
using Microsoft.Test.RERP.Library;
using Microsoft.Test.RERP.Library.Handlers;
using Microsoft.Test.RERP.Library.Handlers.Jobs;
using Microsoft.Test.RERP.Library.Handlers.NotificationRequests;

namespace Microsoft.REPR.UnitTests;

public class ConfigExtensionTests
{

    [Fact]
    public async Task AddREPR_WithValidREPROptionsAndHandler_ShouldAddHandlerTransient()
    {
        // Arrange
        var assemblyName = "Microsoft.Test.RERP.Library";
        AppDomain.CurrentDomain.Load(typeof(TransientHandler).Assembly.GetName());
        var services = new ServiceCollection();
        var reprOptions = new REPROptions
        {
            FilteredAssemblies = [assemblyName],
            IncludeAppDomainAssemblies = true,
        };

        // Act
        services.AddREPR(reprOptions);
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        await AssertTransientHandlers(serviceProvider, services);
        await AssertSingletonHandlers(serviceProvider, services);
        await AssertScopedHandlers(serviceProvider, services);
    }

    [Fact]
    public void AddREPR_WithNoValidHandlersAndNoOptions_ShouldThrowREPRException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var exception = Assert.Throws<REPRException>(() => services.AddREPR());

        // Assert
        Assert.Equal(REPRConstants.NoResourcesAddedError, exception.Message);
    }


    [Fact]
    public void AddREPR_WithNoValidHandlersWithOptionToTargetDomainAssemblies_ShouldThrowREPRException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var exception = Assert.Throws<REPRException>(() => services.AddREPR());

        // Assert
        Assert.Equal(REPRConstants.NoResourcesAddedError, exception.Message);
    }

    [Fact]
    public void AddREPR_WithInvalidOptionsIncludeAppDomainInAssembliesCannotBeTrue_ShouldThrowREPRException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var exception = Assert.Throws<REPRException>(() => services.AddREPR(options => options.IncludeAppDomainAssemblies = true));

        // Assert
        Assert.Equal("Including App Domain Assemblies is only supported with adding filtered assemblies.", exception.Message);
    }

    [Fact]
    public void AddREPR_WithInvalidOptionsFilteredAssembliesCantBeEmpty_ShouldThrowREPRException()
    {
        // Arrange
        var services = new ServiceCollection();
        var reprOptions = new REPROptions
        {
            FilteredAssemblies = [],
            IncludeAppDomainAssemblies = true,
        };

        // Act
        var exception = Assert.Throws<REPRException>(() => services.AddREPR(reprOptions));

        // Assert
        Assert.Equal("Including App Domain Assemblies is only supported with adding filtered assemblies.", exception.Message);
    }

    private static async Task AssertTransientHandlers(IServiceProvider serviceProvider, IServiceCollection services)
    {
        AssertHandler<IRequestHandler<TransientHandlerRequest, TransientHandlerResponse>, TransientHandler>(serviceProvider, services, ServiceLifetime.Transient);
        await AssertREPR<TransientHandlerRequest, TransientHandlerResponse>(serviceProvider, new TransientHandlerRequest());

        AssertHandler<IListRequestHandler<TransientListHandlerResponse>, TransientListHandler>(serviceProvider, services, ServiceLifetime.Transient);
        await AssertREPR<TransientListHandlerResponse>(serviceProvider);

        AssertHandler<ICommandQueryRequestHandler<TransientCommandQueryHandlerCommand, TransientCommandQueryHandlerQuery, TransientCommandQueryHandlerResponse>, TransientCommandQueryHandler>(serviceProvider, services, ServiceLifetime.Transient);
        await AssertREPR<TransientCommandQueryHandlerCommand, TransientCommandQueryHandlerQuery, TransientCommandQueryHandlerResponse>(serviceProvider, new TransientCommandQueryHandlerCommand(), new TransientCommandQueryHandlerQuery());

        AssertTwoHandlers<IJobHandler<TransientJobRequest, TransientJobResponse>, TransientJobHandlerOne, TransientJobHandlerTwo>(serviceProvider, services, ServiceLifetime.Transient);
        await AssertREPRJobs<TransientJobRequest, TransientJobResponse>(serviceProvider, new TransientJobRequest());

        AssertTwoHandlers<INotificationHandler<TransientNotificationRequest>, TransientNotificationHandlerOne, TransientNotificationHandlerOne>(serviceProvider, services, ServiceLifetime.Transient);
        await AssertREPRNotifications(serviceProvider, new TransientNotificationRequest());
    }

    private static async Task AssertSingletonHandlers(IServiceProvider serviceProvider, IServiceCollection services)
    {
        AssertHandler<IRequestHandler<SingletonHandlerRequest, SingletonHandlerResponse>, SingletonHandler>(serviceProvider, services, ServiceLifetime.Singleton);
        await AssertREPR<SingletonHandlerRequest, SingletonHandlerResponse>(serviceProvider, new SingletonHandlerRequest());

        AssertHandler<IListRequestHandler<SingletonListHandlerResponse>, SingletonListHandler>(serviceProvider, services, ServiceLifetime.Singleton);
        await AssertREPR<SingletonListHandlerResponse>(serviceProvider);

        AssertHandler<ICommandQueryRequestHandler<SingletonCommandQueryHandlerCommand, SingletonCommandQueryHandlerQuery, SingletonCommandQueryHandlerResponse>, SingletonCommandQueryHandler>(serviceProvider, services, ServiceLifetime.Singleton);
        await AssertREPR<SingletonCommandQueryHandlerCommand, SingletonCommandQueryHandlerQuery, SingletonCommandQueryHandlerResponse>(serviceProvider, new SingletonCommandQueryHandlerCommand(), new SingletonCommandQueryHandlerQuery());

        AssertTwoHandlers<IJobHandler<SingletonJobRequest, SingletonJobResponse>, SingletonJobHandlerOne, SingletonJobHandlerTwo>(serviceProvider, services, ServiceLifetime.Singleton);
        await AssertREPRJobs<SingletonJobRequest, SingletonJobResponse>(serviceProvider, new SingletonJobRequest());

        AssertTwoHandlers<INotificationHandler<SingletonNotificationRequest>, SingletonNotificationHandlerOne, SingletonNotificationHandlerTwo>(serviceProvider, services, ServiceLifetime.Singleton);
        await AssertREPRNotifications(serviceProvider, new SingletonNotificationRequest());
    }

    private static async Task AssertScopedHandlers(IServiceProvider serviceProvider, IServiceCollection services)
    {
        AssertHandler<IRequestHandler<ScopedHandlerRequest, ScopedHandlerResponse>, ScopeHandler>(serviceProvider, services, ServiceLifetime.Scoped);
        await AssertREPR<ScopedHandlerRequest, ScopedHandlerResponse>(serviceProvider, new ScopedHandlerRequest());

        AssertHandler<IListRequestHandler<ScopedListHandlerResponse>, ScopedListHandler>(serviceProvider, services, ServiceLifetime.Scoped);
        await AssertREPR<ScopedListHandlerResponse>(serviceProvider);

        AssertHandler<ICommandQueryRequestHandler<ScopedCommandQueryHandlerCommand, ScopedCommandQueryHandlerQuery, ScopedCommandQueryHandlerResponse>, ScopedCommandQueryHandler>(serviceProvider, services, ServiceLifetime.Scoped);
        await AssertREPR<ScopedCommandQueryHandlerCommand, ScopedCommandQueryHandlerQuery, ScopedCommandQueryHandlerResponse>(serviceProvider, new ScopedCommandQueryHandlerCommand(), new ScopedCommandQueryHandlerQuery());

        AssertTwoHandlers<IJobHandler<ScopedJobRequest, ScopedJobResponse>, ScopedJobHandlerOne, ScopedJobHandlerTwo>(serviceProvider, services, ServiceLifetime.Scoped);
        await AssertREPRJobs<ScopedJobRequest, ScopedJobResponse>(serviceProvider, new ScopedJobRequest());

        AssertTwoHandlers<INotificationHandler<ScopedNotificationRequest>, ScopedNotificationHandlerOne, ScopedNotificationHandlerTwo>(serviceProvider, services, ServiceLifetime.Scoped);
        await AssertREPRNotifications(serviceProvider, new ScopedNotificationRequest());
    }

    private static void AssertHandler<TRequestHandler, THandler>(IServiceProvider serviceProvider, IServiceCollection services, ServiceLifetime serviceLifetime)
    {
        var fullName = typeof(THandler).FullName;
        Assert.NotNull(fullName);
        var requestService = services.Single(s => s.ImplementationType is not null && !string.IsNullOrEmpty(s.ImplementationType.FullName) && s.ImplementationType.FullName.StartsWith(fullName));
        var handler = serviceProvider.GetService<TRequestHandler>();
        Assert.NotNull(handler);
        Assert.Equal(serviceLifetime, requestService.Lifetime);
    }

    private static void AssertTwoHandlers<TRequestHandler, THandler1, THandler2>(IServiceProvider serviceProvider, IServiceCollection services, ServiceLifetime serviceLifetime)
    {
        var fullName1 = typeof(THandler1).FullName;
        Assert.NotNull(fullName1);
        var requestService1 = services.Single(s => s.ImplementationType is not null && !string.IsNullOrEmpty(s.ImplementationType.FullName) && s.ImplementationType.FullName.StartsWith(fullName1));
        Assert.Equal(serviceLifetime, requestService1.Lifetime);

        var fullName2 = typeof(THandler1).FullName;
        Assert.NotNull(fullName2);
        var requestService2 = services.Single(s => s.ImplementationType is not null && !string.IsNullOrEmpty(s.ImplementationType.FullName) && s.ImplementationType.FullName.StartsWith(fullName1));
        Assert.Equal(serviceLifetime, requestService1.Lifetime);

        var ty = typeof(TRequestHandler);
        var handlers = serviceProvider.GetServices<IJobHandler<TransientJobRequest, TransientJobResponse>>();
        Assert.NotNull(handlers);
        Assert.Contains(handlers, h => h.GetType() == typeof(TransientJobHandlerOne));
        Assert.Contains(handlers, h => h.GetType() == typeof(TransientJobHandlerTwo));
    }

    private static async Task AssertREPRJobs<TRequest, TResponse>(IServiceProvider serviceProvider, TRequest request)
    {
        var repr = serviceProvider.GetService<IREPR>();
        Assert.NotNull(repr);
        Assert.IsType<REPR>(repr);
        var responses = await repr.ExecuteJob<TRequest, TResponse>(request, CancellationToken.None);
        Assert.NotNull(responses);
        Assert.IsAssignableFrom<IEnumerable<TResponse>>(responses);
        foreach (var response in responses)
        {
            Assert.NotNull(response);
        }
    }

    private static async Task AssertREPRNotifications<TRequest>(IServiceProvider serviceProvider, TRequest request)
    {
        var repr = serviceProvider.GetService<IREPR>();
        Assert.NotNull(repr);
        Assert.IsType<REPR>(repr);
        await repr.Send(request, CancellationToken.None);
    }

    private static async Task AssertREPR<TRequest, TResponse>(IServiceProvider serviceProvider, TRequest request)
    {
        var repr = serviceProvider.GetService<IREPR>();
        Assert.NotNull(repr);
        Assert.IsType<REPR>(repr);
        var response = await repr.Handle<TRequest, TResponse>(request, CancellationToken.None);
        Assert.NotNull(response);
        Assert.IsType<TResponse>(response);
    }

    private static async Task AssertREPR<TCommand, TQuery, TResponse>(IServiceProvider serviceProvider, TCommand command, TQuery query)
    {
        var repr = serviceProvider.GetService<IREPR>();
        Assert.NotNull(repr);
        Assert.IsType<REPR>(repr);
        var response = await repr.Handle<TCommand, TQuery, TResponse>(command, query, CancellationToken.None);
        Assert.NotNull(response);
        Assert.IsType<TResponse>(response);
    }

    private static async Task AssertREPR<TResponse>(IServiceProvider serviceProvider)
    {
        var repr = serviceProvider.GetService<IREPR>();
        Assert.NotNull(repr);
        Assert.IsType<REPR>(repr);
        var response = await repr.Handle<TResponse>(CancellationToken.None);
        Assert.NotNull(response);
        Assert.IsAssignableFrom<IEnumerable<TResponse>>(response);
    }
}
