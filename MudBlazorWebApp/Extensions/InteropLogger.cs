using Microsoft.JSInterop;

namespace MudBlazorWebApp.Extensions;

public static class InteropLogger
{
    private static ILogger _logger;

    public static void Configure(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger("InteropLogger");
    }

    [JSInvokable("LogFromJS")]
    public static void LogFromJavaScript(string message)
    {
        _logger?.LogInformation("JS Message: {Message}", message);
        //Console.WriteLine($"JS Message: {message}");
    }
}
