using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using REPR.Models;
using Test.REPR.Library;

namespace REPR.Performance;

[MemoryDiagnoser(true)]
public class REPRBenchmark
{
    private readonly REPROptions _reprOptions;
    public REPRBenchmark()
    {
        var assemblyName = "Test.REPR.Library";
        AppDomain.CurrentDomain.Load(typeof(TransientHandler).Assembly.GetName());
        var services = new ServiceCollection();
        _reprOptions = new REPROptions
        {
            FilteredAssemblies = [assemblyName],
            IncludeAppDomainAssemblies = true,
        };
    }


    [Benchmark]
    public void AddREPR()
    {
        var services = new ServiceCollection();
        services.AddREPR<Program>(_reprOptions);
    }
}
