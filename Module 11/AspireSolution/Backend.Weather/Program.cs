
using Azure.Monitor.OpenTelemetry.AspNetCore;
using Backend.CustomMetrics;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Backend.Weather;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //builder.AddServiceDefaults();

        // Add services to the container.
        // APPLICATIONINSIGHTS_CONNECTION_STRING=@"InstrumentationKey=74775b93-02a0-40c6-9d7c-326325ef42f8;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/;ApplicationId=a4589d5a-b081-4e5c-9a0e-4a7bbf269ae4"
        builder.Services.AddAuthorization();
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(res => res.AddService(OTELDiagnostics.ServiceName))
            .UseAzureMonitor(conf => {
                conf.ConnectionString = @"InstrumentationKey=14cfff9f-681d-4fd5-9161-8442e6926567;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/;ApplicationId=6aacfdc9-086c-4851-9996-66cdc0536145";
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();
                metrics.AddMeter(OTELDiagnostics.Meter.Name);
                metrics.AddOtlpExporter();
            })
            .WithTracing(trace =>
            {
                trace
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();
                trace.AddOtlpExporter();
            });
        builder.Logging.AddOpenTelemetry(log=>log.AddOtlpExporter());

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        //app.MapDefaultEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        var summaries = new List<string>
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weather", (HttpContext httpContext, ILogger<Program> logger) =>
        {
            // logger.LogInformation("Begin get /weather");
            logger.GetWeatherRequest(Hook.Start);
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = summaries[Random.Shared.Next(summaries.Count)]
                })
                .ToArray();
            logger.GetWeatherRequest(Hook.End);
            //logger.LogInformation("End get /weather");
            OTELDiagnostics.WeatherRequest.Add(1,
                new KeyValuePair<string, object?>("weather.user", "Patrick"),
                new KeyValuePair<string, object?>("weather.time", DateTime.Now.ToShortTimeString())
                );
            return forecast;
        })
        .WithName("GetWeather")
        .WithOpenApi();

        app.MapPost("/weather", (HttpContext ctx, [FromBody] WeatherForecast weather) => {
            if (weather != null && weather.Summary != null)
            {
                summaries.Add(weather.Summary);
                return Results.Accepted("/weather", weather.Summary);
            }
            return Results.BadRequest("Invalid Weatherforecast");
        })
        .WithName("PostWeather")
        .WithOpenApi();

        app.Run();
    }
}
