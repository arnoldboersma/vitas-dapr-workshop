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
        return await Task.FromResult(new SummaryRequest
        {
            Id = Guid.NewGuid(),
            Url = newSummarizeRequest.Url,
            UrlHashed = GetHashedUrl(newSummarizeRequest.Url),
            Email = newSummarizeRequest.Email,
            Summary = "",
        });
    }

    internal async Task<List<SummaryRequest>> GetSummaryRequestsAsync(CancellationToken ct = default)
    {
        return await Task.FromResult(new List<SummaryRequest>{new() {
            Id = Guid.NewGuid(),
            Url = "",
            UrlHashed = "",
            Email = "",
            Summary = "",
        }});
    }

    internal async Task<SummaryRequest?> SearchSummaryRequestByUrlAsync(SearcRequest searcRequest, CancellationToken ct = default)
    {
        return await Task.FromResult<SummaryRequest?>(null);
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
