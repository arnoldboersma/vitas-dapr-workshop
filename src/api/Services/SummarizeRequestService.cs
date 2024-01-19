using System.Security.Cryptography;
using System.Text;
using Api.Models;
using Dapr.Client;

namespace Api.Services;

public class SummarizeRequestService(DaprClient daprClient, AppSettings appSettings)
{
    private readonly DaprClient daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
    private readonly AppSettings appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

    internal async Task<SummaryRequest> CreateSummaryRequestAsync(NewSummarizeRequest newSummarizeRequest, CancellationToken ct = default)
    {
        Dictionary<string, string> metadata = new() { { "contentType", "application/json" } };
        var request = new SummaryRequest
        {
            Id = Guid.NewGuid(),
            Email = newSummarizeRequest.Email,
            Url = newSummarizeRequest.Url,
            UrlHashed = GetHashedUrl(newSummarizeRequest.Url),
            Summary = newSummarizeRequest.Summary,
        };

        // Save State
        await this.daprClient.SaveStateAsync(appSettings.StateStoreName, request.Id.ToString(), request, metadata: metadata, cancellationToken: ct);

        return request;
    }

    internal async Task<List<SummaryRequest>> GetSummaryRequestsAsync(CancellationToken ct = default)
    {
        var query = """
        {
            "page": {
                "limit": 100
            }
        }
        """;

        // Get State
        Dictionary<string, string> metadata = new() { { "contentType", "application/json" }, { "queryIndexName", this.appSettings.StateStoreQueryIndexName } };
        var queryResult = await daprClient.QueryStateAsync<SummaryRequest>(appSettings.StateStoreName, query, cancellationToken: ct, metadata: metadata);
        return queryResult.Results.Select(s => s.Data).ToList();
    }

    internal async Task<SummaryRequest?> SearchSummaryRequestByUrlAsync(SearcRequest searcRequest, CancellationToken ct = default)
    {
        var query = $$"""
        {
            "page": {
                "limit": 100
            },
            "filter": {
                "EQ": {
                    "url_hashed": "{{GetHashedUrl(searcRequest.Url)}}"
                }
            }
        }
        """;

        // Search Summary Request by URL
        Dictionary<string, string> metadata = new() { { "contentType", "application/json" }, { "queryIndexName", this.appSettings.StateStoreQueryIndexName } };
        var queryResult = await daprClient.QueryStateAsync<SummaryRequest>(appSettings.StateStoreName, query, cancellationToken: ct, metadata: metadata);
        return queryResult.Results.Select(s => s.Data).FirstOrDefault();
    }

    private static string GetHashedUrl(string url)
    {
        var encoding = new System.Text.UTF8Encoding();
        byte[] keyByte = encoding.GetBytes("secretkey");
        byte[] dataBytes = encoding.GetBytes(url);

        using var hmacsha256 = new HMACSHA256(keyByte);
        byte[] hashmessage = hmacsha256.ComputeHash(dataBytes);
        return BitConverter.ToString(hashmessage).Replace("-", "").ToLower();
    }
}
