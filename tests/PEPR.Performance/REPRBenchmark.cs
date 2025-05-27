using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.REPR.Models;
using Microsoft.Test.RERP.Library;

namespace Microsoft.REPR.Performance;

[MemoryDiagnoser(true)]
public class REPRBenchmark
{
    private readonly REPROptions _reprOptions;
    public REPRBenchmark()
    {
        var assemblyName = "Microsoft.Test.RERP.Library";
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
        services.AddREPR(_reprOptions);
    }
}
