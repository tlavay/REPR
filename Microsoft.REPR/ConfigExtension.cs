using Microsoft.Extensions.DependencyInjection;
using Microsoft.REPR.Models;
using Microsoft.REPR.Utilities;

namespace Microsoft.REPR;

public static class ConfigExtension
{
    public static void AddREPR(this IServiceCollection services)
    {
        var defaultREPROptions = new REPROptions
        {
            FilteredAssemblies = null,
            StrictMode = true,
        };

        REPRUtilities.AddREPRInternal(ref services, defaultREPROptions);
    }

    public static void AddREPR(this IServiceCollection services, Action<REPROptions> reprOptions)
    {
        var newOptions = new REPROptions();
        reprOptions(newOptions);
        REPRUtilities.AddREPRInternal(ref services, newOptions);
    }

    public static void AddREPR(this IServiceCollection services, REPROptions reprOptions)
    {
        REPRUtilities.AddREPRInternal(ref services, reprOptions);
    }
}
