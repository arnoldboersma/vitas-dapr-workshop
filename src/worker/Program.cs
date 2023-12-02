using Worker;
using Dapr;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.MapPost("/orders", [Topic("summarizer-pubsub", "link-to-summarize")] (NewSummaryRequestPayload newSummaryRequestPayload) => {
    Console.WriteLine("Subscriber received : " + newSummaryRequestPayload);
    return Results.Ok(newSummaryRequestPayload);
})
.WithOpenApi();;

app.Run();
