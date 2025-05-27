using Microsoft.Extensions.DependencyInjection;
using Microsoft.REPR.Constants;
using Microsoft.REPR.Exceptions;
using Microsoft.REPR.Models;

namespace Microsoft.REPR.Utilities;

internal static class REPRUtilities
{
    public static void AddREPRInternal(ref IServiceCollection services, in REPROptions reprOptions)
    {
        var targetExecutingAssembly = reprOptions.FilteredAssemblies is not null && reprOptions.FilteredAssemblies.Any();
        if (reprOptions.IncludeAppDomainAssemblies && !targetExecutingAssembly)
        {
            throw new REPRException("Including App Domain Assemblies is only supported with adding filtered assemblies.");
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
