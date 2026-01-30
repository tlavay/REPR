using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using REPR;

namespace REPR.SourceGeneration.Tests;

public class REPRGeneratorTests
{
    [Fact]
    public void Generator_WithNoHandlers_GeneratesEmptyRegistration()
    {
        // Arrange
        var source = @"
namespace TestApp
{
    public class Program
    {
        public static void Main() { }
    }
}";

        // Act
        var (diagnostics, output) = RunGenerator(source);

        // Assert
        Assert.Empty(diagnostics);
        Assert.Single(output);
        var generatedCode = output[0].SourceText.ToString();
        Assert.Contains("public static class REPRRegistration", generatedCode);
        Assert.Contains("public static IServiceCollection AddREPRHandlers", generatedCode);
        Assert.Contains("services.AddTransient<global::REPR.IREPR, global::REPR.REPR>()", generatedCode);
    }

    [Fact]
    public void Generator_WithTransientHandler_GeneratesTransientRegistration()
    {
        // Arrange
        var source = @"
using System.Threading;
using System.Threading.Tasks;
using REPR.Handlers;

namespace TestApp
{
    public record TestRequest;
    public record TestResponse;

    internal sealed class TestHandler : ITransientRequestHandler<TestRequest, TestResponse>
    {
        public Task<TestResponse> Handle(TestRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new TestResponse());
        }
    }
}";

        // Act
        var (diagnostics, output) = RunGenerator(source);

        // Assert
        Assert.Empty(diagnostics);
        Assert.Single(output);
        var generatedCode = output[0].SourceText.ToString();
        Assert.Contains("// Transient handlers", generatedCode);
        Assert.Contains("services.AddTransient<global::REPR.Handlers.IRequestHandler<global::TestApp.TestRequest, global::TestApp.TestResponse>, global::TestApp.TestHandler>()", generatedCode);
    }

    [Fact]
    public void Generator_WithScopedHandler_GeneratesScopedRegistration()
    {
        // Arrange
        var source = @"
using System.Threading;
using System.Threading.Tasks;
using REPR.Handlers;

namespace TestApp
{
    public record ScopedRequest;
    public record ScopedResponse;

    internal sealed class ScopedTestHandler : IScopedRequestHandler<ScopedRequest, ScopedResponse>
    {
        public Task<ScopedResponse> Handle(ScopedRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ScopedResponse());
        }
    }
}";

        // Act
        var (diagnostics, output) = RunGenerator(source);

        // Assert
        Assert.Empty(diagnostics);
        Assert.Single(output);
        var generatedCode = output[0].SourceText.ToString();
        Assert.Contains("// Scoped handlers", generatedCode);
        Assert.Contains("services.AddScoped<global::REPR.Handlers.IRequestHandler<global::TestApp.ScopedRequest, global::TestApp.ScopedResponse>, global::TestApp.ScopedTestHandler>()", generatedCode);
    }

    [Fact]
    public void Generator_WithSingletonHandler_GeneratesSingletonRegistration()
    {
        // Arrange
        var source = @"
using System.Threading;
using System.Threading.Tasks;
using REPR.Handlers;

namespace TestApp
{
    public record SingletonRequest;
    public record SingletonResponse;

    internal sealed class SingletonTestHandler : ISingletonRequestHandler<SingletonRequest, SingletonResponse>
    {
        public Task<SingletonResponse> Handle(SingletonRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new SingletonResponse());
        }
    }
}";

        // Act
        var (diagnostics, output) = RunGenerator(source);

        // Assert
        Assert.Empty(diagnostics);
        Assert.Single(output);
        var generatedCode = output[0].SourceText.ToString();
        Assert.Contains("// Singleton handlers", generatedCode);
        Assert.Contains("services.AddSingleton<global::REPR.Handlers.IRequestHandler<global::TestApp.SingletonRequest, global::TestApp.SingletonResponse>, global::TestApp.SingletonTestHandler>()", generatedCode);
    }

    [Fact]
    public void Generator_WithMultipleHandlers_GroupsByLifetimeAndSortsAlphabetically()
    {
        // Arrange
        var source = @"
using System.Threading;
using System.Threading.Tasks;
using REPR.Handlers;

namespace TestApp
{
    public record RequestA;
    public record ResponseA;
    public record RequestB;
    public record ResponseB;
    public record RequestC;
    public record ResponseC;

    // Intentionally out of alphabetical order
    internal sealed class ZetaHandler : ITransientRequestHandler<RequestA, ResponseA>
    {
        public Task<ResponseA> Handle(RequestA request, CancellationToken cancellationToken) => Task.FromResult(new ResponseA());
    }

    internal sealed class AlphaHandler : ITransientRequestHandler<RequestB, ResponseB>
    {
        public Task<ResponseB> Handle(RequestB request, CancellationToken cancellationToken) => Task.FromResult(new ResponseB());
    }

    internal sealed class BetaHandler : IScopedRequestHandler<RequestC, ResponseC>
    {
        public Task<ResponseC> Handle(RequestC request, CancellationToken cancellationToken) => Task.FromResult(new ResponseC());
    }
}";

        // Act
        var (diagnostics, output) = RunGenerator(source);

        // Assert
        Assert.Empty(diagnostics);
        Assert.Single(output);
        var generatedCode = output[0].SourceText.ToString();

        // Scoped should come before Transient (grouped by lifecycle)
        var scopedIndex = generatedCode.IndexOf("// Scoped handlers");
        var transientIndex = generatedCode.IndexOf("// Transient handlers");
        Assert.True(scopedIndex < transientIndex, "Scoped handlers should be grouped before Transient handlers");

        // Within Transient, AlphaHandler should come before ZetaHandler (alphabetical)
        var alphaIndex = generatedCode.IndexOf("AlphaHandler");
        var zetaIndex = generatedCode.IndexOf("ZetaHandler");
        Assert.True(alphaIndex < zetaIndex, "Handlers should be sorted alphabetically within their lifecycle group");
    }

    [Fact]
    public void Generator_WithJobHandler_GeneratesCorrectRegistration()
    {
        // Arrange
        var source = @"
using System.Threading;
using System.Threading.Tasks;
using REPR.Handlers;

namespace TestApp
{
    public record JobRequest;
    public record JobResponse;

    internal sealed class TestJobHandler : IScopedJobHandler<JobRequest, JobResponse>
    {
        public Task<JobResponse> Execute(JobRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new JobResponse());
        }
    }
}";

        // Act
        var (diagnostics, output) = RunGenerator(source);

        // Assert
        Assert.Empty(diagnostics);
        Assert.Single(output);
        var generatedCode = output[0].SourceText.ToString();
        Assert.Contains("services.AddScoped<global::REPR.Handlers.IJobHandler<global::TestApp.JobRequest, global::TestApp.JobResponse>, global::TestApp.TestJobHandler>()", generatedCode);
    }

    [Fact]
    public void Generator_WithNotificationHandler_GeneratesCorrectRegistration()
    {
        // Arrange
        var source = @"
using System.Threading;
using System.Threading.Tasks;
using REPR.Handlers;

namespace TestApp
{
    public record TestNotification;

    internal sealed class TestNotificationHandler : ITransientNotificationHandler<TestNotification>
    {
        public Task Send(TestNotification request, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}";

        // Act
        var (diagnostics, output) = RunGenerator(source);

        // Assert
        Assert.Empty(diagnostics);
        Assert.Single(output);
        var generatedCode = output[0].SourceText.ToString();
        Assert.Contains("services.AddTransient<global::REPR.Handlers.INotificationHandler<global::TestApp.TestNotification>, global::TestApp.TestNotificationHandler>()", generatedCode);
    }

    [Fact]
    public void Generator_WithListHandler_GeneratesCorrectRegistration()
    {
        // Arrange
        var source = @"
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using REPR.Handlers;

namespace TestApp
{
    public record ListItemResponse;

    internal sealed class TestListHandler : ISingletonListRequestHandler<ListItemResponse>
    {
        public Task<IEnumerable<ListItemResponse>> Handle(CancellationToken cancellationToken)
        {
            return Task.FromResult<IEnumerable<ListItemResponse>>(new List<ListItemResponse>());
        }
    }
}";

        // Act
        var (diagnostics, output) = RunGenerator(source);

        // Assert
        Assert.Empty(diagnostics);
        Assert.Single(output);
        var generatedCode = output[0].SourceText.ToString();
        Assert.Contains("// Singleton handlers", generatedCode);
        Assert.Contains("services.AddSingleton<global::REPR.Handlers.IListRequestHandler<global::TestApp.ListItemResponse>, global::TestApp.TestListHandler>()", generatedCode);
    }

    [Fact]
    public void Generator_WithCommandQueryHandler_GeneratesCorrectRegistration()
    {
        // Arrange
        var source = @"
using System.Threading;
using System.Threading.Tasks;
using REPR.Handlers;

namespace TestApp
{
    public record TestCommand;
    public record TestQuery;
    public record TestCQResponse;

    internal sealed class TestCommandQueryHandler : IScopedCommandQueryRequestHandler<TestCommand, TestQuery, TestCQResponse>
    {
        public Task<TestCQResponse> Handle(TestCommand command, TestQuery query, CancellationToken cancellationToken)
        {
            return Task.FromResult(new TestCQResponse());
        }
    }
}";

        // Act
        var (diagnostics, output) = RunGenerator(source);

        // Assert
        Assert.Empty(diagnostics);
        Assert.Single(output);
        var generatedCode = output[0].SourceText.ToString();
        Assert.Contains("services.AddScoped<global::REPR.Handlers.ICommandQueryRequestHandler<global::TestApp.TestCommand, global::TestApp.TestQuery, global::TestApp.TestCQResponse>, global::TestApp.TestCommandQueryHandler>()", generatedCode);
    }

    [Fact]
    public void Generator_WithAbstractClass_DoesNotGenerateRegistration()
    {
        // Arrange
        var source = @"
using System.Threading;
using System.Threading.Tasks;
using REPR.Handlers;

namespace TestApp
{
    public record TestRequest;
    public record TestResponse;

    internal abstract class AbstractHandler : ITransientRequestHandler<TestRequest, TestResponse>
    {
        public abstract Task<TestResponse> Handle(TestRequest request, CancellationToken cancellationToken);
    }
}";

        // Act
        var (diagnostics, output) = RunGenerator(source);

        // Assert
        Assert.Empty(diagnostics);
        Assert.Single(output);
        var generatedCode = output[0].SourceText.ToString();
        Assert.DoesNotContain("AbstractHandler", generatedCode);
    }

    [Fact]
    public void Generator_AlwaysRegistersIREPRAsTransient()
    {
        // Arrange
        var source = @"
namespace TestApp
{
    public class Program { }
}";

        // Act
        var (diagnostics, output) = RunGenerator(source);

        // Assert
        Assert.Empty(diagnostics);
        Assert.Single(output);
        var generatedCode = output[0].SourceText.ToString();
        Assert.Contains("// Register REPR mediator as transient", generatedCode);
        Assert.Contains("services.AddTransient<global::REPR.IREPR, global::REPR.REPR>()", generatedCode);
    }

    private static (ImmutableArray<Diagnostic> Diagnostics, ImmutableArray<GeneratedSourceResult> Output) RunGenerator(string source)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Task).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(CancellationToken).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(IEnumerable<>).Assembly.Location),
            MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
        };

        // Add REPR assembly reference
        var reprAssembly = typeof(global::REPR.IREPR).Assembly;
        references.Add(MetadataReference.CreateFromFile(reprAssembly.Location));

        var compilation = CSharpCompilation.Create(
            "TestAssembly",
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var generator = new REPRGenerator();

        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        var runResult = driver.GetRunResult();

        return (diagnostics, runResult.Results.SelectMany(r => r.GeneratedSources).ToImmutableArray());
    }
}
