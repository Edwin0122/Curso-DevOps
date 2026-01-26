using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Swagger (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger SOLO fuera de Testing
if (!app.Environment.IsEnvironment("Testing"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contactos API v1");
        c.RoutePrefix = "swagger";
    });
}

// HTTPS solo fuera de Testing/Docker
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Endpoint
app.MapGet("/weatherforecast", () =>
{
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild",
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

    // 🔑 SERIALIZACIÓN MANUAL (clave para .NET 9 + TestHost)
    var json = JsonSerializer.Serialize(forecast);

    return Results.Content(json, "application/json");
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Necesario para WebApplicationFactory
public partial class Program { }
