
using System;
using System.Threading.Tasks;

#region KeyVault
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Azure.Security.KeyVault.Secrets;
using Azure.Security.KeyVault;
#endregion

#region AppConfiguration
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Microsoft.Extensions.Azure;
using Azure.Core;
#endregion


namespace KeyVault
{
    class Program
    {
        static string tenentId = "030b09d5-7f0f-40b0-8c01-03ac319b2d71";
        static string clientId = "06a90d24-bc74-40b5-ad73-1f5c06bf9355";
        static string clientSecret = "5cN8Q~G~TP.ILSrge9GLMtzA55bFxSH4nXSAEcch";
        static string kvUri = "https://ps-kluisjes.vault.azure.net/";
        
        static async Task Main(string[] args)
        {
            //clientSecret = Environment.GetEnvironmentVariable("GEHEIM");
           //await ReadKeyVault();
           //await ReadConfigurationLocalAsync();
          await ReadAppConfigurationAsync();

            Console.WriteLine("Done");
            Console.ReadLine();
        }
        private static async Task ReadKeyVault()
        {
            //ClientSecretCredential cred = new ClientSecretCredential(tenentId, clientId, clientSecret);
            DefaultAzureCredential cred = new DefaultAzureCredential();
            //var cred = new ManagedIdentityCredential("49cd35d4-b9e5-49ad-b477-0711710afb1a");
            SecretClient kvClient = new SecretClient(new Uri(kvUri), cred);

            var result = await kvClient.GetSecretAsync("MySecret");
            Console.WriteLine($"Hello: {result.Value?.Value}");
        }
        private static Task ReadConfigurationLocalAsync()
        {
            // From user-secrets
            // dotnet init
            // dotnet user-secrets set APPC "Endpoint=https://ps-config.azconfig.io;Id=LDpu;Secret=zE8JVpCLRSBwkKyE+sHBE1uPnexZTenYGpvY2eQoY04="
            // var constr = configuration["APPC"];
            // Console.WriteLine(  constr);
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json")
                   .AddUserSecrets<KeyVault.Program>(true)
                   .AddEnvironmentVariables();
            IConfiguration configuration = builder.Build();

            Console.WriteLine(configuration["MySetings:hello"]);
            Console.WriteLine(configuration["KlantA:KeyVault:BackgroundColor"]);
            Console.WriteLine(configuration["ConnectionString"]);
            return Task.CompletedTask;
        }

        private static async Task ReadAppConfigurationAsync()
        {
            var builder = new ConfigurationBuilder();
            //ClientSecretCredential cred = new ClientSecretCredential(tenentId, clientId, clientSecret);
            //var env = Environment.GetEnvironmentVariable("Bla");
            // builder.AddAzureKeyVault(new Uri(kvUri), cred);

            builder.AddAzureAppConfiguration(opts =>
            {
                opts.ConfigureKeyVault(kvopts =>
                {
                    kvopts.SetCredential(new DefaultAzureCredential());
                });
                //Console.WriteLine(Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT"));
                opts.Connect("Endpoint=https://ps-config.azconfig.io;Id=Xyxn;Secret=I014kK0XDWbG11aGYlsh63+fFHC/Zky1yQXoyDfuLiU=")
                    .Select(KeyFilter.Any, "Production")
                     .TrimKeyPrefix("KeyVault:");
                //opts.ConfigureRefresh(refr => refr.Register("*", true));
                opts.UseFeatureFlags(conf =>
                {
                    conf.Select(KeyFilter.Any, "Production");
                    conf.CacheExpirationInterval = TimeSpan.FromSeconds(10);
                });
            }); 
                IConfiguration conf = builder.Build();
                
                Console.WriteLine($"{conf["MySetings:hello"]}");
            Console.WriteLine($"{conf["MySetings:secret"]}");
                //Console.WriteLine($"Hello {conf["ConnectionString"]}");
              //  Console.WriteLine(conf["KlantA:KeyVault:BackgroundColor"]);
               Console.WriteLine($"{conf["KV"]}");

                IServiceCollection services = new ServiceCollection();
                services.AddFeatureManagement(conf);
                //services.AddSingleton<IConfiguration>(conf).AddFeatureManagement();

                using (var svcProvider = services.BuildServiceProvider())
                {
                    do
                    {
                        using (var scope = svcProvider.CreateScope())
                        {
                            var featureManager = scope.ServiceProvider.GetRequiredService<IFeatureManager>();

                            if (await featureManager.IsEnabledAsync("FeatureA"))
                            {
                                Console.WriteLine("We have a new feature");
                            }
                            Console.Write(".");
                            await Task.Delay(2000);
                        }
                    }
                    while (true);
                }
            }

        private static async Task ReadAppFeaturesAsync()
        {
            var builder = new ConfigurationBuilder();
            builder.AddAzureAppConfiguration(opts => {
                opts.Connect("Endpoint=https://ps-config.azconfig.io;Id=wqyc;Secret=Dkmh9tT9TsWUqK9XWlCH6eoI+I5szowYmkzXkuCO5Zo=")
                   .UseFeatureFlags(opts =>
                   {
                       opts.CacheExpirationInterval = TimeSpan.FromSeconds(1);
                       opts.Label = "Production";
                   });
            });
            IConfiguration conf = builder.Build();

            IServiceCollection services = new ServiceCollection();
            services.AddFeatureManagement(conf);
            services.AddSingleton<IConfiguration>(conf).AddFeatureManagement();

            using (var svcProvider = services.BuildServiceProvider())
            {
                do
                {
                    using (var scope = svcProvider.CreateScope())
                    {
                        var featureManager = scope.ServiceProvider.GetRequiredService<IFeatureManager>();

                        if (await featureManager.IsEnabledAsync("FeatureA"))
                        {
                            Console.WriteLine("We have a new feature");
                        }
                        Console.Write(".");
                        await Task.Delay(2000);
                    }
                }
                while (true);
            }
        }
    }
}
