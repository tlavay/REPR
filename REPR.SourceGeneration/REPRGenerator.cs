using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace REPR;

[Generator]
public class REPRGenerator : IIncrementalGenerator
{
    // Handler interface names mapped to their service lifetime
    private static readonly Dictionary<string, string> SingletonHandlers = new()
    {
        ["ISingletonRequestHandler"] = "IRequestHandler",
        ["ISingletonJobHandler"] = "IJobHandler",
        ["ISingletonCommandQueryRequestHandler"] = "ICommandQueryRequestHandler",
        ["ISingletonListRequestHandler"] = "IListRequestHandler",
        ["ISingletonNotificationHandler"] = "INotificationHandler",
    };

    private static readonly Dictionary<string, string> ScopedHandlers = new()
    {
        ["IScopedRequestHandler"] = "IRequestHandler",
        ["IScopedJobHandler"] = "IJobHandler",
        ["IScopedCommandQueryRequestHandler"] = "ICommandQueryRequestHandler",
        ["IScopedListRequestHandler"] = "IListRequestHandler",
        ["IScopedNotificationHandler"] = "INotificationHandler",
    };

    private static readonly Dictionary<string, string> TransientHandlers = new()
    {
        ["ITransientRequestHandler"] = "IRequestHandler",
        ["ITransientJobHandler"] = "IJobHandler",
        ["ITransientCommandQueryRequestHandler"] = "ICommandQueryRequestHandler",
        ["ITransientListRequestHandler"] = "IListRequestHandler",
        ["ITransientNotificationHandler"] = "INotificationHandler",
        // Base interfaces default to transient
        ["IRequestHandler"] = "IRequestHandler",
        ["IJobHandler"] = "IJobHandler",
        ["ICommandQueryRequestHandler"] = "ICommandQueryRequestHandler",
        ["IListRequestHandler"] = "IListRequestHandler",
        ["INotificationHandler"] = "INotificationHandler",
    };

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Find all class declarations that implement REPR handler interfaces
        var handlerDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) => IsHandlerCandidate(node),
                transform: static (ctx, _) => GetHandlerInfo(ctx))
            .Where(static info => info is not null)
            .Select(static (info, _) => info!.Value);

        // Combine all handlers and generate the registration code
        var compilationAndHandlers = context.CompilationProvider.Combine(handlerDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndHandlers, static (spc, source) =>
        {
            var (compilation, handlers) = source;
            Execute(spc, compilation, handlers);
        });
    }

    private static bool IsHandlerCandidate(SyntaxNode node)
    {
        // Only consider class declarations with base list (implementing interfaces)
        return node is ClassDeclarationSyntax classDecl &&
               classDecl.BaseList is not null &&
               classDecl.BaseList.Types.Count > 0;
    }

    private static HandlerInfo? GetHandlerInfo(GeneratorSyntaxContext context)
    {
        var classDecl = (ClassDeclarationSyntax)context.Node;
        var semanticModel = context.SemanticModel;

        var classSymbol = semanticModel.GetDeclaredSymbol(classDecl);
        if (classSymbol is null || classSymbol.IsAbstract)
        {
            return null;
        }

        // Check all interfaces the class implements
        foreach (var iface in classSymbol.AllInterfaces)
        {
            var interfaceName = iface.OriginalDefinition.Name;
            var fullInterfaceName = iface.OriginalDefinition.ToDisplayString();

            // Check if this is a REPR handler interface
            if (!fullInterfaceName.StartsWith("REPR.Handlers.", StringComparison.Ordinal))
            {
                continue;
            }

            // Determine lifetime and base interface
            string? lifetime = null;
            string? baseInterface = null;

            if (SingletonHandlers.TryGetValue(interfaceName, out baseInterface))
            {
                lifetime = "Singleton";
            }
            else if (ScopedHandlers.TryGetValue(interfaceName, out baseInterface))
            {
                lifetime = "Scoped";
            }
            else if (TransientHandlers.TryGetValue(interfaceName, out baseInterface))
            {
                lifetime = "Transient";
            }

            if (lifetime is not null && baseInterface is not null)
            {
                // Get the generic type arguments for the interface
                var typeArgs = iface.TypeArguments;
                var typeArgsString = string.Join(", ", typeArgs.Select(t => t.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)));

                return new HandlerInfo
                {
                    ClassName = classSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    InterfaceName = $"global::REPR.Handlers.{baseInterface}<{typeArgsString}>",
                    Lifetime = lifetime,
                    SortKey = classSymbol.Name
                };
            }
        }

        return null;
    }

    private static void Execute(SourceProductionContext context, Compilation compilation, ImmutableArray<HandlerInfo> handlers)
    {
        if (handlers.IsDefaultOrEmpty)
        {
            // Generate empty registration method
            var emptySource = GenerateSource(
                Enumerable.Empty<HandlerInfo>(),
                Enumerable.Empty<HandlerInfo>(),
                Enumerable.Empty<HandlerInfo>());
            context.AddSource("REPRRegistration.g.cs", SourceText.From(emptySource, Encoding.UTF8));
            return;
        }

        // Group by lifetime and sort alphabetically within each group
        var singletonHandlers = handlers
            .Where(h => h.Lifetime == "Singleton")
            .OrderBy(h => h.SortKey, StringComparer.Ordinal)
            .ToList();

        var scopedHandlers = handlers
            .Where(h => h.Lifetime == "Scoped")
            .OrderBy(h => h.SortKey, StringComparer.Ordinal)
            .ToList();

        var transientHandlers = handlers
            .Where(h => h.Lifetime == "Transient")
            .OrderBy(h => h.SortKey, StringComparer.Ordinal)
            .ToList();

        var source = GenerateSource(singletonHandlers, scopedHandlers, transientHandlers);
        context.AddSource("REPRRegistration.g.cs", SourceText.From(source, Encoding.UTF8));
    }

    private static string GenerateSource(
        IEnumerable<HandlerInfo> singletonHandlers,
        IEnumerable<HandlerInfo> scopedHandlers,
        IEnumerable<HandlerInfo> transientHandlers)
    {
        var sb = new StringBuilder();

        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine();
        sb.AppendLine("using Microsoft.Extensions.DependencyInjection;");
        sb.AppendLine();
        sb.AppendLine("namespace REPR;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Source-generated REPR handler registration. No runtime reflection required.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public static class REPRRegistration");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Registers all REPR handlers discovered at compile time.");
        sb.AppendLine("    /// Handlers are grouped by lifecycle and sorted alphabetically.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static IServiceCollection AddREPRHandlers(this IServiceCollection services)");
        sb.AppendLine("    {");

        // Singleton handlers
        if (singletonHandlers.Any())
        {
            sb.AppendLine("        // Singleton handlers");
            foreach (var handler in singletonHandlers)
            {
                sb.AppendLine($"        services.AddSingleton<{handler.InterfaceName}, {handler.ClassName}>();");
            }
            sb.AppendLine();
        }

        // Scoped handlers
        if (scopedHandlers.Any())
        {
            sb.AppendLine("        // Scoped handlers");
            foreach (var handler in scopedHandlers)
            {
                sb.AppendLine($"        services.AddScoped<{handler.InterfaceName}, {handler.ClassName}>();");
            }
            sb.AppendLine();
        }

        // Transient handlers
        if (transientHandlers.Any())
        {
            sb.AppendLine("        // Transient handlers");
            foreach (var handler in transientHandlers)
            {
                sb.AppendLine($"        services.AddTransient<{handler.InterfaceName}, {handler.ClassName}>();");
            }
            sb.AppendLine();
        }

        sb.AppendLine("        // Register REPR mediator as transient");
        sb.AppendLine("        services.AddTransient<global::REPR.IREPR, global::REPR.REPR>();");
        sb.AppendLine();
        sb.AppendLine("        return services;");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

    private struct HandlerInfo
    {
        public string ClassName;
        public string InterfaceName;
        public string Lifetime;
        public string SortKey;
    }
}
