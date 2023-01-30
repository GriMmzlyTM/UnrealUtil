namespace UnrealUtils.Models;

/// <summary>
/// JSON POCO for Unreal .target files
/// </summary>
public record UnrealTarget {
    
    /// <summary>
    /// The name of the application 
    /// </summary>
    public string TargetName { get; init; }
    public PlatformEnum Platform { get; init; }
    public ConfigurationEnum Configuration { get; init; }
    public TargetTypeEnum TargetType { get; init; }
    public bool IsTestTarget { get; init; }
    
    /// <summary>
    /// The executable of the application
    /// </summary>
    public string Launch { get; init; }
    
    /// <summary>
    /// App specific files
    /// </summary>
    public List<UnrealObject> BuildProducts { get; init; }
    
    /// <summary>
    /// Dependency files (Unreal files)
    /// </summary>
    public List<UnrealObject> RuntimeDependencies { get; init; }

    /// <summary>
    /// Get all paths in <see cref="BuildProducts"/> and <see cref="RuntimeDependencies"/>
    /// </summary>
    /// <returns></returns>
    public List<UnrealObject> GetAllDirectories() {
        var allDirs = new List<UnrealObject>(BuildProducts);
        allDirs.AddRange(RuntimeDependencies);

        return allDirs;
    }
}
