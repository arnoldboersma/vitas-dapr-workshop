using System.Text.Json.Serialization;

namespace Api.Models;

public record SummaryRequest
{
    [JsonPropertyName("email")]
    public required string Email { get; init; }

    [JsonPropertyName("url")]
    public required string Url { get; init; }

    [JsonPropertyName("url_hashed")]
    public required string UrlHashed { get; init; }

    [JsonPropertyName("summary")]
    public required string Summary { get; init; }

    [JsonPropertyName("id")]
    public required Guid Id { get; init; }
}