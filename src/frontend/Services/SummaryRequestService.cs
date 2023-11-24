namespace frontend.Services;

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Dapr.Client;

public class SummaryRequestService
{
    private readonly DaprClient _daprClient;

    private readonly ILogger<SummaryRequestService> _logger;

    public SummaryRequestService(DaprClient daprClient, ILogger<SummaryRequestService> Logger)
    {
        this._daprClient = daprClient;
        this._logger = Logger;
    }

    public async Task<string> GetSummaryRequestsAsync()
    {
        var order = new Order(2);
        var orderJson = JsonSerializer.Serialize(order);
        var content = new StringContent(orderJson, Encoding.UTF8, "application/json");

        HttpRequestMessage? response = this._daprClient.CreateInvokeMethodRequest(
            HttpMethod.Post,
            "summarizer-api",
            "/orders", 
            content
        );
        return await this._daprClient.InvokeMethodAsync<string>(response);
    }
}

public record Order([property: JsonPropertyName("orderId")] int OrderId);