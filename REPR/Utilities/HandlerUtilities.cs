using Microsoft.Extensions.DependencyInjection;
using REPR.Exceptions;

namespace REPR.Utilities;

internal static class HandlerUtilities
{
    public static bool AddHandlers(this IServiceCollection services, in List<Type> targetTypes, bool strictMode)
    {
        var handlersAdded = false;
        var targetHandlers = targetTypes.Where(targetType => targetType is not null && AssemblyUtility.GetREPRRequestHandlers(targetType, strictMode)).ToArray();
        foreach (var targetHandler in targetHandlers)
        {
            var reprHandlerDetail = GetREPRHandlerDetail(targetHandler.GetInterfaces(), targetHandler.BaseType!);
            switch (reprHandlerDetail.ServiceLifetime)
            {
                case ServiceLifetime.Transient:
                    services.AddTransient(reprHandlerDetail.RequestHandlerInterfaceType, targetHandler);
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped(reprHandlerDetail.RequestHandlerInterfaceType, targetHandler);
                    break;
                case ServiceLifetime.Singleton:
                    services.AddSingleton(reprHandlerDetail.RequestHandlerInterfaceType, targetHandler);
                    break;
                default:
                    throw new REPRException($"The REPR Service Lifetime: '{reprHandlerDetail.ServiceLifetime}' is not supported for type: '{targetHandler.FullName}'. This error should not occur. Please create a bug.");
            }

            handlersAdded = true;
        }

        return handlersAdded;
    }

    private static REPRHandlerDetail GetREPRHandlerDetail(in Type[] interfaces, in Type baseType)
    {
        Type? baseRequestHandler = null;
        ServiceLifetime? minServiceLifetime = null;
        foreach (var i in interfaces)
        {
            if (AssemblyUtility.InterfaceIsRequestHandler(i.FullName))
            {
                var requestHandlerServiceLifetime = AssemblyUtility.GetRequestHandlerServiceLifetime(i.FullName!);
                if (minServiceLifetime is null || requestHandlerServiceLifetime < minServiceLifetime)
                {
                    minServiceLifetime = requestHandlerServiceLifetime;
                }

                if (AssemblyUtility.IsBaseHandler(i.FullName))
                {
                    baseRequestHandler = i;
                }
            }
        }

        if (baseRequestHandler is null || minServiceLifetime is null)
        {
            throw new REPRException($"No service lifetime or IRequestHandler interface could be  found for type: '{baseType.FullName}'. This exception should not be possible. Create a bug with REPR team.");
        }

        return new REPRHandlerDetail
        {
            RequestHandlerInterfaceType = baseRequestHandler,
            ServiceLifetime = minServiceLifetime.Value
        };
    }

    private sealed record REPRHandlerDetail
    {
        public required Type RequestHandlerInterfaceType { get; init; }
        public required ServiceLifetime ServiceLifetime { get; init; }
    }
}
