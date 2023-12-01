namespace Frontend.Services;
using Dapr.Client;

public class SummaryRequestService(DaprClient daprClient, AppSettings settingsService, ILogger<SummaryRequestService> Logger)
{
    private readonly DaprClient _daprClient = daprClient;
    private readonly AppSettings _settings = settingsService;
    private readonly ILogger<SummaryRequestService> _logger = Logger;

    public async Task AddSummaryRequestAsync(NewSummaryRequestPayload newSummaryRequest)
    {
        CancellationTokenSource source = new CancellationTokenSource();
        CancellationToken cancellationToken = source.Token;

        await this._daprClient.PublishEventAsync(
            _settings.PubRequestName,
            _settings.PubRequestTopic,
            newSummaryRequest,
            cancellationToken
        );
    }

    public async Task<SummaryRequest[]> GetSummaryRequestsAsync()
    {
        HttpRequestMessage? response = this._daprClient.CreateInvokeMethodRequest(
            HttpMethod.Get,
            _settings.requestsApiAppId,
            _settings.requestsApiEndpoint
        );
        return await this._daprClient.InvokeMethodAsync<SummaryRequest[]>(response);
    }
}
