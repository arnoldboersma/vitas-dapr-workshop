using System.Text.Json.Serialization;

namespace Api.Models;

public record NewSummarizeRequest
{
    [JsonPropertyName("email")]
    public string Email { get; set; } = default!;

    [JsonPropertyName("url")]

    public string Url { get; set; } = default!;

    [JsonPropertyName("summary")]
    public string Summary { get; set; } = default!;
}
