using Api.Models;
using Dapr.Client;

namespace Api.Services;

public class SummarizeRequestService(DaprClient daprClient, AppSettings appSettings)
{
    private readonly DaprClient daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
    private readonly AppSettings appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

    public async Task<SummaryRequest> CreateSummaryRequestAsync(NewSummarizeRequest newSummarizeRequest)
    {
        Dictionary<string, string> metadata = new() { { "contentType", "application/json" } };
        var request = new SummaryRequest
        {
            Id = Guid.NewGuid(),
            Email = newSummarizeRequest.Email,
            Url = newSummarizeRequest.Url,
            UrlHashed = newSummarizeRequest.Url.GetHashCode().ToString(),
            Summary = newSummarizeRequest.Summary,
        };

        await this.daprClient.SaveStateAsync(appSettings.StateStoreName, request.Id.ToString(), request, metadata: metadata);

        return request;
    }

    public async Task<List<SummaryRequest>> GetSummaryRequestsAsync(CancellationToken ct = default)
    {
        var query1 = """
        {
            "page": {
                "limit": 100
            }
        }
        """;

        var query2 = """
        {
            "page": {
                "limit": 100
            },
            "filter": {
                "EQ": {
                    "url_hashed": "774448577"
                }
            }
        }
        """;

        Dictionary<string, string> metadata = new() { { "contentType", "application/json" }, { "queryIndexName", this.appSettings.StateStoreQueryIndexName } };
        var queryResult = await daprClient.QueryStateAsync<SummaryRequest>(appSettings.StateStoreName, query1, cancellationToken: ct, metadata: metadata);
        return queryResult.Results.Select(s => s.Data).ToList();
    }


}
