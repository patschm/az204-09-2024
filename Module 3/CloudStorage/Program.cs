using Azure;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using System.Reflection;

namespace CloudStorageV12;

class Program
{
    private const string connectionString = "DefaultEndpointsProtocol=https;AccountName=psstoor;AccountKey=cJvQAh+r8+1Ygiz776jFSXewhYe5ESflmr/cflNYJai6Bobe/V2haQLRqucyHPK2VIXvnOO2Tv/P+ASt22XmYQ==;EndpointSuffix=core.windows.net";
    private static string basePath = Assembly.GetExecutingAssembly().Location;

    static async Task Main(string[] args)
    {
        basePath = basePath.Substring(0, basePath.IndexOf("\\bin") + 1);
        //WriteSimple();
        //ReadSimple();
        //WriteBlockBlob();
        //await LeaseAsync();
        //await MiscAsync();
        

        // Optional. 
        WritePageBlob();
        //ReadPageBlob();
        //await ArchiveAsync();
        Console.WriteLine("Press Enter to quit");
        Console.ReadLine();
    }



    private static BlobServiceClient CreateAccount(BlobClientOptions options = null)
    {
        BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString, options);
        return blobServiceClient;
    }
    private static BlobContainerClient GetContainer(string containerName)
    {
        BlobServiceClient account = CreateAccount();
        BlobContainerClient container = account.GetBlobContainerClient(containerName);
        container.CreateIfNotExists();
        return container;
    }

    private static void WriteSimple()
    {
        BlobContainerClient container = GetContainer("demo");
        BlobClient blob = container.GetBlobClient("castle0.jpg");
        
        using (FileStream fs = File.OpenRead(basePath + "chambord.jpg"))
        {
            blob.Upload(fs, overwrite: true);
        }
    }
    private static void ReadSimple()
    {
        BlobContainerClient container = GetContainer("demo");
        BlobClient blob = container.GetBlobClient("castle0.jpg");
        BlobDownloadInfo inf = blob.Download();
        Console.WriteLine(inf.Details.ContentType);
        using (FileStream fs = File.OpenWrite(basePath + "chambord10.jpg"))
        {
            inf.Content.CopyTo(fs);
        }
    }
    private static void WriteBlockBlob()
    {
        BlobContainerClient container = GetContainer("demo");
        BlockBlobClient blob = container.GetBlockBlobClient("castle1.jpg");

        List<string> blockList = new List<string>();

        int blockID = 1;
        int blockSize = 1_000_000;
        using (FileStream fs = File.OpenRead(basePath + "chambord.jpg"))
        {
            using (BinaryReader rdr = new BinaryReader(fs))
            {
                byte[] buffer;
                do
                {
                    buffer = rdr.ReadBytes(blockSize);
                    using (MemoryStream mem = new MemoryStream(buffer))
                    {
                        // Block id's must have the same length
                        string sBlockId = Convert.ToBase64String(BitConverter.GetBytes(blockID));
                        blob.StageBlock(sBlockId, mem, null);
                        blockList.Add(sBlockId);
                        blockID++;
                    }
                }
                while (buffer.Length == blockSize);
            }
        }
        blob.CommitBlockList(blockList);
    }
    private static async Task LeaseAsync()
    {
        var container = GetContainer("lease");
        var client = container.GetBlobClient("lease_castle0.jpg");
        using (FileStream fs = File.OpenRead(basePath + "chambord.jpg"))
        {
            await client.UploadAsync(fs);
        }

        var leaseClient = client.GetBlobLeaseClient();
        var lease = await leaseClient.AcquireAsync(TimeSpan.FromSeconds(59));
        Console.WriteLine($"You can have this blob for {lease.Value.LeaseTime} seconds");

        var leaseClient2 = client.GetBlobLeaseClient();
        try
        {
            var lease2 = await leaseClient2.AcquireAsync(TimeSpan.FromSeconds(30));
        }
        catch (RequestFailedException ex)
        {
            Console.WriteLine(ex.ErrorCode);
            Console.WriteLine("Blob already in use");
        }

        Console.WriteLine("Press Enter to release lease");
        Console.ReadLine();
        await leaseClient.ReleaseAsync();
        //await leaseClient.RenewAsync();

    }

    private static async Task MiscAsync()
    {
        var container = GetContainer("pub");

        var permissions = await container.GetAccessPolicyAsync();
        Console.WriteLine(permissions.Value.BlobPublicAccess);
        await container.SetAccessPolicyAsync(PublicAccessType.BlobContainer);

        BlobClient blob = container.GetBlobClient("castle100.jpg");
        using (FileStream fs = File.OpenRead(basePath + "chambord.jpg"))
        {
            blob.Upload(fs, true);
        }


        await blob.SetMetadataAsync(new Dictionary<string, string> { { "meta_tag", "hello" }, { "meta_help", "ok" } });

        BlobProperties props = await blob.GetPropertiesAsync();
        //props.
        Console.WriteLine($"Properties: Type: {props.ContentType}, Size: {props.ContentLength}");
        Console.WriteLine($"Metadata:");
        foreach (var meta in props.Metadata)
        {
            Console.WriteLine($"{meta.Key}: {meta.Value}");
        }


        var options = new BlobClientOptions();
        options.Retry.Mode = RetryMode.Fixed;
        options.Retry.Delay = TimeSpan.FromSeconds(60);
        options.Retry.MaxRetries = 3;
  
        
        var acct = CreateAccount(options);
        var blob2 = acct.GetBlobContainerClient("pub").GetBlobClient("castle1.jpg");

        using (FileStream fs = File.OpenRead(basePath + "chambord.jpg"))
        {
            await blob2.UploadAsync(fs);
        }
    }

    
    private static async Task ArchiveAsync()
    {
        var container = GetContainer("archive");
        var client = container.GetBlobClient("castle0.jpg");
        client.SetAccessTier(AccessTier.Archive);

        using (FileStream fs = File.OpenRead(basePath + "chambord.jpg"))
        {
            await client.UploadAsync(fs);
        }
    }
    private static void WritePageBlob()
    {
        BlobContainerClient container = GetContainer("demo");
        PageBlobClient blob = container.GetPageBlobClient("plaatgeheugen.vhd");
        
        // must be a multitude of 512
        int byteCount = 1048576;
        using (FileStream fs = File.OpenRead(basePath + "chambord.jpg"))
        {
            blob.Create(1000 * byteCount);

            using (BinaryReader rdr = new BinaryReader(fs))
            {
                int offset = 0;
                byte[] buffer;
               // do
                {
                    buffer = rdr.ReadBytes(18000000);
                    Array.Resize(ref buffer, byteCount);
                    using (MemoryStream mem = new MemoryStream(buffer))
                    {
                        blob.UploadPages(mem, offset);
                        //offset += 1 * byteCount;
                    }
                }
                //while (offset < fs.Length);
            }
        }
    }
    private static void ReadPageBlob()
    {
        BlobContainerClient container = GetContainer("demo");
        PageBlobClient blob = container.GetPageBlobClient("castle2.jpg");

        var pageRanges = blob.GetPageRanges()?.Value?.PageRanges;
        using (FileStream fs = File.OpenWrite(basePath + "chambord2.jpg"))
        {
            foreach (var range in pageRanges)
            {
                using (var strm = blob.OpenRead(range.Offset))
                {
                    strm.CopyTo(fs);
                }
            }
        }
    }
}
