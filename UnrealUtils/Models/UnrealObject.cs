
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UnrealUtils.Models;

[JsonConverter(typeof(StringEnumConverter))]
public enum ObjectType {
    RequiredResource,
    DynamicLibrary,
    SymbolFile,
    Executable,

    NonUFS,
    UFS,
    DebugNonUFS,
}

public record UnrealObject {
    public string Path { get; init; }
    public ObjectType Type { get; init; }
}
