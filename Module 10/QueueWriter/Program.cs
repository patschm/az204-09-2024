using Azure;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace QueueWriter;

class Program
{
    static string EndPoint = "ps-endpoint.servicebus.windows.net";
    static (string Name, string Key) SasKeyManager = ("RootManageSharedAccessKey","V7CAvAexUPQ38rs/vI03cKo4xgUctnmL7HuUlkLva+4=");
    static (string Name, string Key)  SasKeyWriter = ("Writer", "sY42vtezJKpZ3/s/eIrHghwcwi9FXIliP+ASbOgQj5A=");
    static string QueueName = "myqueue";

    static async Task Main(string[] args)
    {
        //await ManageQueueAsync();
        await WriteToQueueAsync();
        Console.WriteLine("Press Enter to Quit");
        Console.ReadLine();
    }

    private static async Task WriteToQueueAsync()
    {
        var cred = new AzureNamedKeyCredential(SasKeyWriter.Name, SasKeyWriter.Key);
        var client = new ServiceBusClient(EndPoint, cred);
        var sender = client.CreateSender(QueueName);
        //var sender = client.Cre
        
        int i = 0;
        ConsoleKey key;
        do
        {
            var msg = new ServiceBusMessage(BinaryData.FromString("Hello World " + (++i).ToString()));
            //msg.SessionId = "me";
            msg.ContentType = "string";
            msg.TimeToLive = TimeSpan.FromSeconds(30);
            //msg.ReplyTo
            await sender.SendMessageAsync(msg);
            Console.WriteLine("Any key to send another message, Esc to quit");
            key = Console.ReadKey().Key;
        }
        while (key != ConsoleKey.Escape);

        var msgb = new ServiceBusMessage(BinaryData.FromString("Bye!!"));
        msgb.ContentType = "string";
        msgb.TimeToLive = TimeSpan.FromSeconds(30);
        await sender.SendMessageAsync(msgb);
    }

    private static async Task ManageQueueAsync()
    {
        var cred = new AzureNamedKeyCredential(SasKeyManager.Name, SasKeyManager.Key);
        var client = new ServiceBusAdministrationClient(EndPoint, cred);
        var queueInfo = new CreateQueueOptions(QueueName);

        var response = await client.CreateQueueAsync(queueInfo);
        if (response != null)
        {
            Console.WriteLine("Queue created!");
        }
    }
}
