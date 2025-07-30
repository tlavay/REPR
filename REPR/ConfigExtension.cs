using Microsoft.Extensions.DependencyInjection;
using REPR.Models;
using REPR.Utilities;

namespace REPR;

public static class ConfigExtension
{
    /// <summary>
    /// Adds REPR handlers to Service Collection.
    /// </summary>
    /// <typeparam name="T">A class where this assembly is called. Use program.cs where possible.</typeparam>
    /// <param name="services">The service Collection.</param>
    /// <exception cref="REPRException">Thrown if the provided source assembly is not valid.</exception>
    public static void AddREPR<T>(this IServiceCollection services)
    {
        var reprOptions = new REPROptions()
        {
            FilteredAssemblies = [],
        };

        REPRUtilities.AddREPRInternal<T>(ref services, reprOptions);
    }

    /// <summary>
    /// Adds REPR handlers to Service Collection.
    /// </summary>
    /// <typeparam name="T">A class where this assembly is called. Use program.cs where possible.</typeparam>
    /// <param name="services">The service Collection.</param>
    /// <exception cref="REPRException">Thrown if the provided source assembly is not valid.</exception>
    public static void AddREPR<T>(this IServiceCollection services, Action<REPROptions> reprOptions)
    {
        var newOptions = new REPROptions() { FilteredAssemblies = [], };
        reprOptions(newOptions);
        REPRUtilities.AddREPRInternal<T>(ref services, newOptions);
    }

    public static void AddREPR<T>(this IServiceCollection services, REPROptions reprOptions)
    {
        REPRUtilities.AddREPRInternal<T>(ref services, reprOptions);
    }
}
