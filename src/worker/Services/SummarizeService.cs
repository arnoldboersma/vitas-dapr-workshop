using Azure;
using Azure.AI.OpenAI;
using Dapr.Client;
using Worker.Models;

namespace Worker.Services;

public class SummarizeService(DaprClient daprClient, AppSettings appSettings)
{
    private readonly DaprClient daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
    private readonly AppSettings appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

    public async Task<string> SummarizeAsync(NewSummaryRequestPayload newSummaryRequestPayload, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Subscriber received 2 : " + newSummaryRequestPayload);
        var summary = await GetSummarry(newSummaryRequestPayload, cancellationToken);
        Console.WriteLine($"Summary: {summary}");
        // var summaryResponse = new SummaryResponse()
        // {
        //     Id = Guid.NewGuid().ToString(),
        //     Url = newSummaryRequestPayload.Url,
        //     Summary = "This is a summary"
        // };

        // await daprClient.PublishEventAsync(appSettings.PubSubRequestsName, appSettings.PubSubRequestsTopic, summaryResponse);

        return summary;
    }

    private async Task<string> GetSummarry(NewSummaryRequestPayload newSummaryRequestPayload, CancellationToken cancellationToken = default)
    {
        // read from secret store
        var apiKey = await daprClient.GetSecretAsync(appSettings.SecretStoreName, "OPENAI-API-KEY", cancellationToken: cancellationToken);
        var apiEndPoint = await daprClient.GetSecretAsync(appSettings.SecretStoreName, "OPENAI-API-ENDPOINT", cancellationToken: cancellationToken);

        apiKey.TryGetValue("OPENAI-API-KEY", out string? key);
        apiEndPoint.TryGetValue("OPENAI-API-ENDPOINT", out string? endPoint);

        if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(endPoint))
        {
            return "OpenAI API Key or Endpoint is missing";
        }

        var openAIClient = new OpenAIClient(new Uri(endPoint), new AzureKeyCredential(key));
        var url = newSummaryRequestPayload.Url;
        string summarizationPrompt = @$"
            Summarize the article {url} in english in less than two paragraphs without adding new information. When the summary seems too short to make at least one paragraph, answer that you can't summarize a text that is too short
        ";

        CompletionsOptions completionsOptions = new()
        {
            DeploymentName = appSettings.OpenAIDeploymentName,
            Prompts = { summarizationPrompt },
            MaxTokens = 100,
            Temperature = 1f,
            NucleusSamplingFactor = 0.5f,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };
        var completionsResponse  = await
            openAIClient.GetCompletionsAsync(completionsOptions, cancellationToken);
        string completion = completionsResponse.Value.Choices[0].Text;
        return completion ?? "No summary found";
    }
}
