namespace Microsoft.REPR.Models;

public sealed record REPROptions
{
    public bool IncludeAppDomainAssemblies { get; set; }
    public IEnumerable<string>? FilteredAssemblies { get; set; }
    public bool StrictMode { get; set; }
}
