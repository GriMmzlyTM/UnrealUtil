using CommandLine;

namespace UnrealUtils.Options; 

/// <summary>
/// Base options 
/// </summary>
public class Options {
            
    [Option(Default = false)]
    public bool Verbose { get; set; }
            
}
