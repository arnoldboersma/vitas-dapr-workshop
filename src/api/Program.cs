
using System.Text.Json;
using Api;
using Api.Models;
using Api.Services;
using Microsoft.ApplicationInsights.Extensibility;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.Configure<TelemetryConfiguration>((o) =>
{
    o.TelemetryInitializers.Add(new AppInsightsTelemetryInitializer());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<SummarizeRequestService>();
builder.Services.AddSingleton<AppSettings>();

builder.Services.AddDaprClient(client =>
{
    client.UseJsonSerializationOptions(new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    });
});

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/healthz");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!")
.WithOpenApi();

app.MapGet("/requests", async (SummarizeRequestService summarizeRequestService, CancellationToken token) =>
{
    var results = await summarizeRequestService.GetSummaryRequestsAsync(token);
    app.Logger.LogInformation("Returning result {result}", results?.Count);
    return Results.Ok(results);
})
.WithOpenApi();

app.MapPost("/requests", async (NewSummarizeRequest newSummarizeRequest, SummarizeRequestService summarizeRequestService, CancellationToken token) =>
{
    var result = await summarizeRequestService.CreateSummaryRequestAsync(newSummarizeRequest, token);
    app.Logger.LogInformation("Returning result {result}", result);
    return Results.Created($"/requests/{result.Id}", result);
})
.WithOpenApi();

app.MapPost("/search-requests-by-url", async (SearcRequest searcRequest, SummarizeRequestService summarizeRequestService, CancellationToken token) =>
{
    var result = await summarizeRequestService.SearchSummaryRequestByUrlAsync(searcRequest, token);
    app.Logger.LogInformation("Returning result {result}", result?.Id);

    if (result is null)
    {
        return Results.NoContent();
    }
    else
    {
        return Results.Ok(result);
    }
})
.WithOpenApi();

app.Run();
