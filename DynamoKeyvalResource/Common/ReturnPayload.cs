using System.Text.Json.Serialization;

namespace Common;

public class ReturnPayload
{
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyName("created")]
    public string? Created { get; set; }

    [JsonIgnore]
    public DateTime? CreatedDateTimeOffset => !string.IsNullOrWhiteSpace(Created) ? Convert.ToDateTime(Created) : null;
}