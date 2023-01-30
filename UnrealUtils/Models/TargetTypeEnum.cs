
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace UnrealUtils.Models; 

[JsonConverter(typeof(StringEnumConverter))]
public enum TargetTypeEnum {
    Program,
}
