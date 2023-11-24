var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/orders", (Order order) =>
{
    Console.WriteLine("Order received : " + order);
    return order;
});

app.Run();
public record Order(int orderId);