using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure.Functions.Worker;
using System.IO;

namespace AzureFunctions;

public static class StoreFunction
{
    public class Person : TableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    // const string conStr = "DefaultEndpointsProtocol=https;AccountName=psmyfuncstorage;AccountKey=jZMof+ntMgA6HPEZf5zPabtvIlXP7VSLyA+OYlnIHeIzSzvRDj/esyfgdAmvHFQpfcqJYlPTS9PL+AStdQAURg==;EndpointSuffix=core.windows.net";


    [return: Table("people", Connection = "CONSTR")]
    [FunctionName("StoreFunction")]
    public static Person Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "people/{first}/{last}/{age?}")] HttpRequest req,
        string first,
        string last,
        int? age,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        return new Person
        {
            PartitionKey = "viahttp",
            RowKey = Guid.NewGuid().ToString(),
            FirstName = first,
            LastName = last,
            Age = age ?? 42
        };
    }

    [FunctionName("ServiceBusOutput")]
    [return: ServiceBus("myqueue", Connection = "ServiceBusConnection")]
    public static string ServiceBusOutput([HttpTrigger] dynamic input, ILogger log)
    {
        log.LogInformation($"C# function processed: {input.Text}");
        return input.Text;
    }

    [FunctionName("BlobOutput")]
    //[BlobOutput("output/{name}-out.txt")]
    public static string BlobOutputting(
        [BlobTrigger("container/{name}")] string myTriggerItem,
        //[BlobInput("test-samples-input/sample1.txt")] string myBlob,
        [Blob("input/{name}", FileAccess.Write)] Stream imageSmall,
        [Blob("output/{name}", FileAccess.Write)] Stream imageMedium,
         FunctionContext context)
    {
        var logger = context.GetLogger("BlobFunction");
        logger.LogInformation("Triggered Item = {myTriggerItem}", myTriggerItem);

        // Blob Output
        return "blob-output content";
    }
}
