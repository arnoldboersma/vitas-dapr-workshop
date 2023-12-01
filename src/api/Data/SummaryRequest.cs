using System.Text.Json.Serialization;

namespace api;

public record SummaryRequest
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("url")]

    public string? Url { get; set; }

    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }
}