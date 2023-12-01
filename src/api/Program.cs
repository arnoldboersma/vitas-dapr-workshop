using api;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

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
});

app.Run();
