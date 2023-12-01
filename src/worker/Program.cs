using Worker;
using Dapr;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Dapr will send serialized event object vs. being raw CloudEvent
app.UseCloudEvents();

// needed for Dapr pub/sub routing
app.MapSubscribeHandler();

app.MapGet("/", () => "Hello World!");

app.MapPost("/orders", [Topic("summarizer-pubsub", "link-to-summarize")] (NewSummaryRequestPayload newSummaryRequestPayload) => {
    Console.WriteLine("Subscriber received : " + newSummaryRequestPayload);
    return Results.Ok(newSummaryRequestPayload);
});

app.Run();
