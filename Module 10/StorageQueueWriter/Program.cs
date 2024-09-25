using Azure;
using Azure.Storage.Queues;

namespace StorageQueueWriter;

class Program
{
    static string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=psstoor;AccountKey=/qY7ApBAGaDgCKFU340VNfu3mIHAHRV2kOHA3yuUGIXWW5NOUdhKPNCAshYVe4atbI8DZT39Z1xX+AStsM+fug==;EndpointSuffix=core.windows.net";
    static string QueueName = "jumbo";
    static async Task Main(string[] args)
    {
        await WriteToQueueAsync();
        Console.WriteLine("Press Enter to Quit");
        Console.ReadLine();
    }

    private static async Task WriteToQueueAsync()
    {
        //var token = new AzureSasCredential("?sv=2022-11-02&ss=q&srt=so&sp=wacu&se=2023-12-15T16:37:48Z&st=2023-12-15T08:37:48Z&spr=https&sig=1sfowMX1S0pSF9QFAq%2BCGb%2B%2BeRyfX2RKRnePnfQpSYw%3D");     
        //var client = new QueueClient(Uri("https://psstoring.queue.core.windows.net/kassa"), token);
        var client = new QueueClient(ConnectionString, QueueName);
        for (int i = 0; i < 100; i++)
        {
            await client.SendMessageAsync($"Hello Number {i}");
        }
    }

}
