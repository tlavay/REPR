using Microsoft.Extensions.DependencyInjection;
using REPR.Exceptions;
using REPR.Models;
using REPR.Utilities;

namespace REPR;

public static class ConfigExtension
{
    private const string GeneratedRegistrationTypeName = "REPR.REPRRegistration";
    private const string GeneratedRegistrationMethodName = "AddREPRHandlers";

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

        AddREPRInternal<T>(services, reprOptions);
    }

    /// <summary>
    /// Adds REPR handlers to Service Collection.
    /// </summary>
    /// <typeparam name="T">A class where this assembly is called. Use program.cs where possible.</typeparam>
    /// <param name="services">The service Collection.</param>
    /// <param name="reprOptions">Configuration options for REPR.</param>
    /// <exception cref="REPRException">Thrown if the provided source assembly is not valid.</exception>
    public static void AddREPR<T>(this IServiceCollection services, Action<REPROptions> reprOptions)
    {
        var newOptions = new REPROptions() { FilteredAssemblies = [], };
        reprOptions(newOptions);
        AddREPRInternal<T>(services, newOptions);
    }

    /// <summary>
    /// Adds REPR handlers to Service Collection.
    /// </summary>
    /// <typeparam name="T">A class where this assembly is called. Use program.cs where possible.</typeparam>
    /// <param name="services">The service Collection.</param>
    /// <param name="reprOptions">Configuration options for REPR.</param>
    /// <exception cref="REPRException">Thrown if the provided source assembly is not valid.</exception>
    public static void AddREPR<T>(this IServiceCollection services, REPROptions reprOptions)
    {
        AddREPRInternal<T>(services, reprOptions);
    }

    private static void AddREPRInternal<T>(IServiceCollection services, REPROptions reprOptions)
    {
        if (reprOptions.UseSourceGeneration)
        {
            AddREPRFromSourceGeneration<T>(services, reprOptions);
        }
        else
        {
            REPRUtilities.AddREPRInternal<T>(ref services, reprOptions);
        }
    }

    private static void AddREPRFromSourceGeneration<T>(IServiceCollection services, REPROptions reprOptions)
    {
        var callingAssembly = typeof(T).Assembly;
        var registrationType = callingAssembly.GetType(GeneratedRegistrationTypeName);
        
        if (registrationType is null)
        {
            throw new REPRException(
                $"Source generation is enabled but the generated registration class '{GeneratedRegistrationTypeName}' was not found. " +
                $"Ensure you have added the REPR.SourceGeneration package as an analyzer: " +
                $"<PackageReference Include=\"REPR.SourceGeneration\" OutputItemType=\"Analyzer\" ReferenceOutputAssembly=\"false\" />");
        }

        var registrationMethod = registrationType.GetMethod(GeneratedRegistrationMethodName, 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        
        if (registrationMethod is null)
        {
            throw new REPRException(
                $"Source generation is enabled but the generated method '{GeneratedRegistrationMethodName}' was not found in '{GeneratedRegistrationTypeName}'. " +
                $"This may indicate a version mismatch between REPR and REPR.SourceGeneration packages.");
        }

        // Invoke the generated registration method
        registrationMethod.Invoke(null, new object[] { services });

        services.AddSingleton(reprOptions);
    }
}
