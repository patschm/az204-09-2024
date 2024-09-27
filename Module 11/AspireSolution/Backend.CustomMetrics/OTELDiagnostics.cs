using System.Diagnostics.Metrics;

namespace Backend.CustomMetrics;

public static class OTELDiagnostics
{
    public const string ServiceName = "Weather";
    public static Meter Meter = new(ServiceName);
    public static Counter<int> WeatherRequest = Meter.CreateCounter<int>("weather.requests");
}
