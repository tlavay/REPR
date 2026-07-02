using Microsoft.Extensions.Logging;

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

    /// <summary>
    /// Optional logger used by REPR. When set together with <see cref="Verbose"/>,
    /// REPR logs the full name of each resolved handler, job, and subscriber type on dispatch.
    /// </summary>
    public ILogger? Logger { get; set; }

    /// <summary>
    /// When true and <see cref="Logger"/> is set, REPR logs the resolved handler/job/subscriber
    /// type full names on dispatch.
    /// </summary>
    public bool Verbose { get; set; }
}
