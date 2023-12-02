using api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/", () => "Hello World!")
.WithOpenApi();

app.MapGet("/requests", () =>
{
    return new SummaryRequest[]
    {
        new()
        {
            Email = "ab@test.com",
            Url = "https://www.test.com",
            Summary = "Test summary",
            Id = "1"
        }
    };
})
.WithOpenApi();

app.Run();
