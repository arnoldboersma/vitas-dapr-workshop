using System.Text.Json.Serialization;

namespace Worker;

public record NewSummaryRequestPayload
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }
}