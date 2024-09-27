using Microsoft.Extensions.Logging;

namespace Backend.CustomMetrics;

public static partial class LoggerExtensions
{
    [LoggerMessage(LogLevel.Information, "{hook} get weather")]
    public static partial void GetWeatherRequest(this ILogger logger, Hook hook);
}

public enum Hook
{
    Start,
    End
}