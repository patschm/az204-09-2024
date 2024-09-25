using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ServerLoos;

public class WebTriggerVersie
{
    private readonly ILogger<WebTriggerVersie> _logger;

    public WebTriggerVersie(ILogger<WebTriggerVersie> logger)
    {
        _logger = logger;
    }

    [Function("WebTriggerVersie")]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="bla/{blaat}")] HttpRequest req,
        string blaat)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult($"Welcome to Azure Functions! {blaat}");
    }

    
}

