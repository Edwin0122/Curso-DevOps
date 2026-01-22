using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Xunit;

public class ProgramTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProgramTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
    {
        builder.UseEnvironment("Testing");
    });
    }

    [Fact]
    public async Task GetWeatherForecast_ReturnsValidResponse()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/weatherforecast");

if (!response.IsSuccessStatusCode)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        throw new Exception($"El servidor falló con: {response.StatusCode}. Detalle: {errorContent}");
    }
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var responseString = await response.Content.ReadAsStringAsync();
        var forecasts = JsonSerializer.Deserialize<WeatherForecast[]>(responseString, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(forecasts);
        Assert.Equal(5, forecasts.Length);

        foreach (var forecast in forecasts)
        {
            Assert.InRange(forecast.TemperatureC, -20, 55);
            Assert.False(string.IsNullOrEmpty(forecast.Summary));
        }
    }

    private record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}