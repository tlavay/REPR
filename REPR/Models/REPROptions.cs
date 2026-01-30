namespace REPR.Models;

public sealed record REPROptions
{
    public bool IncludeAppDomainAssemblies { get; set; }
    public required List<string> FilteredAssemblies { get; set; }
    public bool StrictMode { get; set; }
    
    /// <summary>
    /// When true, uses compile-time source generation for handler registration instead of runtime reflection.
    /// Requires the REPR.SourceGeneration package to be referenced as an analyzer.
    /// </summary>
    public bool UseSourceGeneration { get; set; }
}
