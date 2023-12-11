using System.Security.Cryptography;
using System.Text;
using Api.Models;
using Dapr.Client;

namespace Api.Services;

public class SummarizeRequestService(DaprClient daprClient, AppSettings appSettings)
{
    private readonly DaprClient daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
    private readonly AppSettings appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

    public async Task<SummaryRequest> CreateSummaryRequestAsync(NewSummarizeRequest newSummarizeRequest, CancellationToken ct = default)
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

        await this.daprClient.SaveStateAsync(appSettings.StateStoreName, request.Id.ToString(), request, metadata: metadata, cancellationToken: ct);

        return request;
    }

    public async Task<List<SummaryRequest>> GetSummaryRequestsAsync(CancellationToken ct = default)
    {
        var query = """
        {
            "page": {
                "limit": 100
            }
        }
        """;

        Dictionary<string, string> metadata = new() { { "contentType", "application/json" }, { "queryIndexName", this.appSettings.StateStoreQueryIndexName } };
        var queryResult = await daprClient.QueryStateAsync<SummaryRequest>(appSettings.StateStoreName, query, cancellationToken: ct, metadata: metadata);
        return queryResult.Results.Select(s => s.Data).ToList();
    }

    internal async Task<SummaryRequest?> SearchSummaryRequestByUrlAsync(SearcRequest searcRequest, CancellationToken ct = default)
    {
        Console.WriteLine(searcRequest);

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

        Console.WriteLine(query);

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
