using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using REPR.Exceptions;
using REPR.Handlers;

namespace REPR.Utilities;

internal static class AssemblyUtility
{
    private static readonly string _baseRequestHandler = typeof(IRequestHandler<,>).FullName!;
    private static readonly string _baseJobHandler = typeof(IJobHandler<,>).FullName!;
    private static readonly string _baseNotificationHandler = typeof(INotificationHandler<>).FullName!;
    private static readonly string _baseCommandQueryRequestHandler = typeof(ICommandQueryRequestHandler<,,>).FullName!;
    private static readonly string _baseListRequestHandler = typeof(IListRequestHandler<>).FullName!;

    private static readonly Dictionary<string, ServiceLifetime> _validHandlers = new Dictionary<string, ServiceLifetime>()
    {
        // The base request handler interfaces
        [_baseRequestHandler] = ServiceLifetime.Transient,
        [_baseCommandQueryRequestHandler] = ServiceLifetime.Transient,
        [_baseListRequestHandler] = ServiceLifetime.Transient,
        [_baseJobHandler] = ServiceLifetime.Transient,
        [_baseNotificationHandler] = ServiceLifetime.Transient,

        // The singleton request handler interfaces
        [typeof(ISingletonRequestHandler<,>).FullName!] = ServiceLifetime.Singleton,
        [typeof(ISingletonJobHandler<,>).FullName!] = ServiceLifetime.Singleton,
        [typeof(ISingletonCommandQueryRequestHandler<,,>).FullName!] = ServiceLifetime.Singleton,
        [typeof(ISingletonListRequestHandler<>).FullName!] = ServiceLifetime.Singleton,
        [typeof(ISingletonNotificationHandler<>).FullName!] = ServiceLifetime.Singleton,

        // The scoped request handler interfaces
        [typeof(IScopedRequestHandler<,>).FullName!] = ServiceLifetime.Scoped,
        [typeof(IScopedJobHandler<,>).FullName!] = ServiceLifetime.Scoped,
        [typeof(IScopedCommandQueryRequestHandler<,,>).FullName!] = ServiceLifetime.Scoped,
        [typeof(IScopedListRequestHandler<>).FullName!] = ServiceLifetime.Scoped,
        [typeof(IScopedNotificationHandler<>).FullName!] = ServiceLifetime.Scoped,

        // The transient request handler interfaces
        [typeof(ITransientRequestHandler<,>).FullName!] = ServiceLifetime.Transient,
        [typeof(ITransientJobHandler<,>).FullName!] = ServiceLifetime.Transient,
        [typeof(ITransientCommandQueryRequestHandler<,,>).FullName!] = ServiceLifetime.Transient,
        [typeof(ITransientListRequestHandler<>).FullName!] = ServiceLifetime.Transient,
        [typeof(ITransientNotificationHandler<>).FullName!] = ServiceLifetime.Transient,
    }.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

    // Other REPR assembly names
    private static readonly string _reprAssemblyName = typeof(REPR).Assembly.GetName().Name!;

    public static List<Type> GetTargetTypes(IEnumerable<string>? filteredAssemblies, in bool includeAppDomainAssemblies)
    {
        var filterTargetAssemblies = false;
        if (filteredAssemblies is not null && filteredAssemblies.Any())
        {
            filteredAssemblies = filteredAssemblies.ToArray();
            filterTargetAssemblies = true;
        }

        var targetTypes = new List<Type>();
        var targetAssemblies = GetTargetedAssemblies(includeAppDomainAssemblies).OrderBy(a => a.FullName);
        foreach (var executingAssembly in targetAssemblies)
        {
            foreach (var type in executingAssembly.GetTypes())
            {
                Type currentType = type!;
                if (currentType.FullName?.StartsWith(_reprAssemblyName, StringComparison.Ordinal) == true)
                {
                    continue;
                }

                var isNotTargetAssembly = IsNotTargetAssembly(filteredAssemblies!, currentType.FullName);
                if (filterTargetAssemblies && isNotTargetAssembly)
                {
                    continue;
                }

                targetTypes.Add(currentType);
            }
        }

        return targetTypes;
    }

    public static bool GetREPRRequestHandlers(in Type assemblyType, in bool strictMode)
    {
        if (assemblyType is null)
        {
            return false;
        }

        if (assemblyType.IsGenericType)
        {
            return false;
        }

        var interfaces = assemblyType.GetInterfaces();
        if (interfaces.Length == 0)
        {
            return false;
        }

        var containsHandlerInterface = interfaces.Any(assemblyType => assemblyType is not null && InterfaceIsRequestHandler(assemblyType.FullName));
        if (!containsHandlerInterface)
        {
            return false;
        }


        if (strictMode)
        {
            if (assemblyType.IsPublic)
            {
                throw new REPRException($"The handler {assemblyType.Name} is public. Handlers must be internal.");
            }

            if (!assemblyType.IsSealed)
            {
                throw new REPRException($"The handler {assemblyType.Name} is not sealed. Handlers must be sealed. See: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/sealed");
            }
        }

        return true;
    }

    public static ServiceLifetime GetRequestHandlerServiceLifetime(string? fullName)
    {
        if (string.IsNullOrEmpty(fullName))
        {
            throw new REPRException($"Internal REPR exception. The type did not have a name. This exception should never happen. Please report this to the REPR team.");
        }

        foreach (var validHandler in _validHandlers)
        {
            if (fullName.StartsWith(validHandler.Key, StringComparison.Ordinal))
            {
                return validHandler.Value;
            }
        }

        throw new REPRException($"The type '{fullName}' is not a valid REPR request handler. This exception should never happen. Please report this to the REPR team.");
    }

    public static bool IsBaseHandler(in string? fullName) => fullName switch
    {
        var _ when string.IsNullOrEmpty(fullName) => throw new REPRException($"Internal REPR exception. The type did not have a name. This exception should never happen. Please report this to the REPR team."),
        var _ when fullName.StartsWith(_baseRequestHandler, StringComparison.Ordinal) => true,
        var _ when fullName.StartsWith(_baseJobHandler, StringComparison.Ordinal) => true,
        var _ when fullName.StartsWith(_baseListRequestHandler, StringComparison.Ordinal) => true,
        var _ when fullName.StartsWith(_baseCommandQueryRequestHandler, StringComparison.Ordinal) => true,
        var _ when fullName.StartsWith(_baseNotificationHandler, StringComparison.Ordinal) => true,
        _ => false,
    };

    public static bool InterfaceIsRequestHandler(string? fullName)
    {
        if (string.IsNullOrEmpty(fullName))
        {
            throw new REPRException($"Internal REPR exception. The type did not have a name. This exception should never happen. Please report this to the REPR team.");
        }

        return _validHandlers.Any(_validHandlers => fullName.StartsWith(_validHandlers.Key, StringComparison.Ordinal));
    }

    private static bool IsNotTargetAssembly(in IEnumerable<string> targetAssemblies, string? fullName) => !targetAssemblies.Any(targetAssembly => fullName?.StartsWith(targetAssembly, StringComparison.Ordinal) == true);

    private static Assembly[] GetTargetedAssemblies(in bool useAppDomainAssemblies)
    {
        if (useAppDomainAssemblies)
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        return [Assembly.GetExecutingAssembly()];
    }
}
