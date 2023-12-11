using System.Text.Json.Serialization;

namespace Api.Models;

public record SearcRequest
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;
}
