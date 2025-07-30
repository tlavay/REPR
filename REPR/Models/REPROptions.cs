namespace REPR.Models;

public sealed record REPROptions
{
    public bool IncludeAppDomainAssemblies { get; set; }
    public required List<string> FilteredAssemblies { get; set; }
    public bool StrictMode { get; set; }
}
