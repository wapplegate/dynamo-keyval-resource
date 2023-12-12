using System.Text.Json.Serialization;

namespace Common;

[JsonSerializable(typeof(ConcoursePayload))]
[JsonSerializable(typeof(ReturnPayload))]
[JsonSerializable(typeof(List<ReturnPayload>))]
public partial class SourceGenerationContext : JsonSerializerContext
{
}