using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ServerlessV2
{
    public class Stoor
    {
        private readonly ILogger<Stoor> _logger;

        public Stoor(ILogger<Stoor> logger)
        {
            _logger = logger;
        }

        [Function(nameof(Stoor))]
        public void Run([QueueTrigger("myqueue-items", Connection = "yeyrtrthyrttrtu")] QueueMessage message)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");
        }
    }
}
