using ACME.DataLayer.Documents;
using Microsoft.Azure.Cosmos;


namespace ACME.Frontend.ChangeTrackerConsole;

internal class Program
{
    private const string _connectionString = "AccountEndpoint=https://ps-kosmos.documents.azure.com:443/;AccountKey=I6NtHvyRc2pglTrJt2xBIpQl7d4pv0YhULt5XxXYLs1aoy2elQzfyXG3rfN2wq0mNXdlMCNDh358ACDbev43cg==;";
    private const string _database = "productDB";

    private static CosmosClient cosmosClient = new CosmosClient(_connectionString);

    static async Task Main(string[] args)
    {
        //await PrepareDatabaseAsync();

        // The lease container acts as a state storage and coordinates processing
        // the change feed across multiple workers.
        // The lease container can be stored in the same account as the monitored
        // container or in a separate account.
        var props = new ContainerProperties("leasecontainer", "/id");
        var leaseContainer = await cosmosClient.GetDatabase(_database)
            .CreateContainerIfNotExistsAsync(props, throughput: 400);


        // Setup the Change feed processor to listen for changes in the Metas container.
        // The code that is run when changes are detected is called the delegate
        var monitoringContainer = cosmosClient.GetContainer(_database, CosmosDb.METAS);
        var builder = monitoringContainer.GetChangeFeedProcessorBuilder<BrandDocument>("changesBrands",
            (IReadOnlyCollection<BrandDocument> roc, CancellationToken ct) => {
                // The delegate that handles changes
                Console.WriteLine($"{roc.Count} Received");
                foreach (var item in roc)
                {
                    Console.WriteLine($"[{item.Id}] {item.Name} ({item.Website})");
                }
                return null;
            });

        // Since multiple hosts can run a change feed processors, make sure the instance name is unique
        var proc = builder
            .WithInstanceName("unique_name")
            .WithLeaseContainer(leaseContainer)
            .Build();

        await proc.StartAsync();
        Console.WriteLine("Press enter to stop");
        Console.ReadLine();
        await proc.StopAsync();
    }
    private static async Task PrepareDatabaseAsync()
    {
        await cosmosClient.CreateDatabaseIfNotExistsAsync(_database);
        var tool = new CosmosDb(cosmosClient, _database);
        await tool.PopulateDatabaseAsync();
    }
}