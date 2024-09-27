using Azure.Monitor.OpenTelemetry.AspNetCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Frontend.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddOpenTelemetry()
        .ConfigureResource(res => res.AddService("Web"))
         .UseAzureMonitor(conf => {
             conf.ConnectionString = @"InstrumentationKey=14cfff9f-681d-4fd5-9161-8442e6926567;IngestionEndpoint=https://westeurope-5.in.applicationinsights.azure.com/;LiveEndpoint=https://westeurope.livediagnostics.monitor.azure.com/;ApplicationId=6aacfdc9-086c-4851-9996-66cdc0536145";
         })
        .WithMetrics(metrics =>
        {
            metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();
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
        builder.Logging.AddOpenTelemetry(log => log.AddOtlpExporter());

        builder.Services.AddControllersWithViews();
        builder.Services.AddHttpClient("weather", conf => {
            conf.BaseAddress = new Uri("https://localhost:7244");
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}
