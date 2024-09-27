using Azure;
using Azure.Storage.Queues;

namespace StorageQueueReader;

class Program
{
    static string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=psstoor;AccountKey=/qY7ApBAGaDgCKFU340VNfu3mIHAHRV2kOHA3yuUGIXWW5NOUdhKPNCAshYVe4atbI8DZT39Z1xX+AStsM+fug==;EndpointSuffix=core.windows.net";
    static string QueueName = "jumbo";
    static async Task Main(string[] args)
    {
        await ReadFromQueueAsync();
        Console.WriteLine("Press Enter to Quit");
        Console.ReadLine();
    }

    private static async Task ReadFromQueueAsync()
    {
        var token = new AzureSasCredential("?sv=2022-11-02&ss=q&srt=so&sp=rwdacp&se=2024-09-27T15:45:25Z&st=2024-09-27T07:45:25Z&spr=https&sig=9Dk9zVpfpCilobg60VtcdojtcVIXRP%2BGm6t8q7qV0D0%3D");
        var client = new QueueClient(new Uri("https://psstatehup.queue.core.windows.net/kueueue"), token);
        //var client = new QueueClient(ConnectionString, QueueName);
        int i = 0;
        do
        {
            // 10 seconds "lease" time
            i++;
            var response = await client.ReceiveMessageAsync(TimeSpan.FromSeconds(30));
            if (response.Value == null)
            {
                await Task.Delay(100);
                continue;
            }
            if (i % 10 == 0)
            {
                Console.WriteLine("Oooops");
                continue;
            }
            var msg = response.Value;
            Console.WriteLine(msg.Body.ToString());

            // We need more time to process
            //await client.UpdateMessageAsync(msg.MessageId, msg.PopReceipt, msg.Body, TimeSpan.FromSeconds(30));
            // Don't forget to remove
            await client.DeleteMessageAsync(msg.MessageId, msg.PopReceipt);
        }
        while (true);
    }
}
