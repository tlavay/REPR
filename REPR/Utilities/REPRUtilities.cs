using Microsoft.Extensions.DependencyInjection;
using REPR.Constants;
using REPR.Exceptions;
using REPR.Models;

namespace REPR.Utilities;

internal static class REPRUtilities
{
    public static void AddREPRInternal<T>(ref IServiceCollection services, in REPROptions reprOptions)
    {
        var sourceAssembly = typeof(T).Assembly.GetName().Name;
        if (string.IsNullOrEmpty(sourceAssembly))
        {
            throw new REPRException("The source assembly provided was not valid.");
        }

        if (!reprOptions.FilteredAssemblies.Any(filteredAssembly => filteredAssembly == sourceAssembly))
        {
            reprOptions.FilteredAssemblies.Add(sourceAssembly);
        }

        var validTargetTypes = AssemblyUtility.GetTargetTypes(reprOptions.FilteredAssemblies, reprOptions.IncludeAppDomainAssemblies);
        if (!validTargetTypes.Any())
        {
            throw new REPRException(REPRConstants.NoResourcesAddedError);
        }

        var handlersAdded = services.AddHandlers(validTargetTypes, reprOptions.StrictMode);
        if (!handlersAdded)
        {
            throw new REPRException(REPRConstants.NoResourcesAddedError);
        }

        services.AddSingleton<IREPR, REPR>();
    }
}
