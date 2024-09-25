using System.Net.Http.Headers;
using Microsoft.Identity.Client;

internal class Program
{
    private static string ServiceUrl = "https://localhost:7260/";
    private static async Task Main(string[] args)
    {
        //await DoTheCodeFlowAsync();
        await DoTheCredentialFlowAsync();
        
        Console.ReadLine();
    }

    private static async Task DoTheCodeFlowAsync()
    {
        // To make this work do the following:
        // 1) Create an application registration for platform Mobile and Desktop Application.
        //    This prepares Code Grant Flow
        // 2) Set Redirect Uri to http://localhost (must be http. Port is optional)
        var bld = PublicClientApplicationBuilder
            .Create("06a90d24-bc74-40b5-ad73-1f5c06bf9355")
            .WithAuthority(AzureCloudInstance.AzurePublic, "030b09d5-7f0f-40b0-8c01-03ac319b2d71")
            .WithRedirectUri("http://localhost:9898/");  // http scheme only!

        var app = bld.Build();
        // .AcquireTokenByUsernamePassword
        var token = await app.AcquireTokenInteractive(
            new string[] { "api://06a90d24-bc74-40b5-ad73-1f5c06bf9355/Read" })
            .ExecuteAsync();

        Console.WriteLine(token.AccessToken);

        var client = new HttpClient();
        client.BaseAddress = new Uri(ServiceUrl);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.AccessToken);

        string data = await client.GetStringAsync("weatherforecast");
        Console.WriteLine(data);
    }
    private static async Task DoTheCredentialFlowAsync()
    {
        // To make this work do the following:
        // 1) On the application registration of the webapi define an App Role
        //    for Application. 
        // 2) Create a new Application Registration for the servie app.
        //    a) Certificates & secrets: Generate a new Client Secret
        //    b) API permissions: Add Permission -> My Apis -> Select your webapi registration.
        //    c) Select Application Permissions (if disabled you forgot or wrongly did step 1)
        //    d) Select the roles you defined in webapi registration
        //    e) Grant Admin consent on the newly created permission.
        var app = ConfidentialClientApplicationBuilder
            .Create("06a90d24-bc74-40b5-ad73-1f5c06bf9355")
            .WithTenantId("030b09d5-7f0f-40b0-8c01-03ac319b2d71")
            .WithClientSecret("-rw8Q~fQKmZ4R-ADFA32aluc-Lwjhg~5g-Srlbb5");

        var token = await app.Build()
            .AcquireTokenForClient(
                new string[]{"api://06a90d24-bc74-40b5-ad73-1f5c06bf9355/.default"}) // Api ID Uri from webapi regstration. Add /.default to it
            .ExecuteAsync();
        Console.WriteLine(token.AccessToken);

        var client = new HttpClient();
        client.BaseAddress = new Uri(ServiceUrl);

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token.AccessToken);

        string data = await client.GetStringAsync("weatherforecast");
        Console.WriteLine(data);
    }
}