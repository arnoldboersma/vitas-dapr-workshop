using Worker;
using Dapr;
using System.Text.Json;
using Dapr.Client;
using Worker.Models;
using Worker.Services;
using Microsoft.ApplicationInsights.Extensibility;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.Configure<TelemetryConfiguration>((o) =>
{
    o.TelemetryInitializers.Add(new AppInsightsTelemetryInitializer());
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<SummarizeService>();
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

// Dapr will send serialized event object vs. being raw CloudEvent
app.UseCloudEvents();

// needed for Dapr pub/sub routing
app.MapSubscribeHandler();

app.MapGet("/", () => "Hello World!")
.WithOpenApi();

// todo settings from environment, add to new class


app.MapPost("/summarize", [Topic("summarizer-pubsub", "link-to-summarize")] async (NewSummaryRequestPayload newSummaryRequestPayload, SummarizeService summarizeService) => {
    Console.WriteLine("Subscriber received1 : " + newSummaryRequestPayload);

    var result = await summarizeService.SummarizeAsync(newSummaryRequestPayload);
    Console.WriteLine("Subscriber received3 : " + result);
    return Results.Ok(result);
})
.WithOpenApi();;

app.Run();
