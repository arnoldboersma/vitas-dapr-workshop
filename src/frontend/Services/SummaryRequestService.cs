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
    }

    public async Task<SummaryRequest[]> GetSummaryRequestsAsync()
    {
        return await Task.FromResult(new SummaryRequest[]{new() {
            Id = Guid.NewGuid().ToString(),
            Url = "",
            Email = "",
            Summary = "",
        }});
    }
}
