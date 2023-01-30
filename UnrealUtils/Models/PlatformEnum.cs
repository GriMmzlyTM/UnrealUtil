
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UnrealUtils.Models; 

[JsonConverter(typeof(StringEnumConverter))]
public enum PlatformEnum {
    Win32,
    Win64,
    
    Mac
}
