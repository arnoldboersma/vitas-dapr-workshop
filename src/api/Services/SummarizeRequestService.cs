using Api.Models;
using Dapr.Client;

namespace Api.Services;

public class SummarizeRequestService(DaprClient daprClient, AppSettings appSettings)
{
    private readonly DaprClient daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
    private readonly AppSettings appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

    public async Task<SummaryRequest> CreateSummaryRequestAsync(NewSummarizeRequest newSummarizeRequest)
    {
        var request = new SummaryRequest
        {
            Id = Guid.NewGuid(),
            Email = newSummarizeRequest.Email,
            Url = newSummarizeRequest.Url,
            UrlHashed = newSummarizeRequest.Url.GetHashCode().ToString(),
            Summary = newSummarizeRequest.Summary,
        };

        await this.daprClient.SaveStateAsync(appSettings.StateStoreName, request.Id.ToString(), request);

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

        // todo handle error's and multiple pages of results
        return queryResult.Results.Select(s => s.Data).ToList();
    }


}
