namespace MSALDemo;

internal class Program
{
    static async Task Main(string[] args)
    {
        // Make sure you consented these Graph Permissions in Azure AD
        DemoBase.Scopes = new string[] { "user.read", "profile", "openid", "email", "offline_access" }; // offline_access for refresh token
        DemoBase.TenantID = "030b09d5-7f0f-40b0-8c01-03ac319b2d71";
        DemoBase.ClientID = "0c5d75ca-66aa-479b-b39b-2b3ad6f93831";
        DemoBase.ClientSecret = "syg7Q~Fn3sknzb.ZrRxAqT~OSMRjVY2mzLIhl";

        // AAD Endpoints
        DemoBase.TenantUrl = $"https://login.microsoftonline.com/{DemoBase.TenantID}/oauth2/v2.0/";
        DemoBase.CommonUrl = $"https://login.microsoftonline.com/common/oauth2/v2.0/";
        DemoBase.OrganizationUrl = $"https://login.microsoftonline.com/organizations/oauth2/v2.0/";

        // Register the following ReturnUrls in your Application Registration.
        DemoBase.BaseRedirectUrl = "http://localhost:8080";
        DemoBase.RedirectUrl = $"{DemoBase.BaseRedirectUrl}/console";


        DemoBase demo;
        //demo = new PasswordFlowDemo();
        demo = new CodeFlowDemo();
        //demo = new ImplicitFlowDemo();
       // demo = new ClientCredentialsFlowDemo();
        //demo = new DeviceCodeFlowDemo();
        await demo.DemoWithMSALAsync();

        Console.WriteLine("Done! Enter to continue");
        Console.ReadLine();
    }
}