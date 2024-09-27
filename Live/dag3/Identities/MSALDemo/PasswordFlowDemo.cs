using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System.Text;


namespace MSALDemo;

public class PasswordFlowDemo: DemoBase
{
    public override async Task DemoWithMSALAsync()
    {
        // Not recommended and will be removed from future OAuth specifications
        string? username;
        string? password;
        LoginScreen(out username, out password);

        var result = await PublicClientApplicationBuilder.Create(ClientID)
                                .WithTenantId(TenantID)
                                .Build()
                                .AcquireTokenByUsernamePassword(Scopes, username, password)
                                .ExecuteAsync();
        if (!string.IsNullOrEmpty(result.AccessToken))
        {
            await CallGraphApi(result.AccessToken);
        }
    }
    public override async Task DemoWithoutMSALAsync()
    {
        // Not recommended and will be removed from future OAuth specifications
        string? username;
        string? password;
        LoginScreen(out username, out password);
        
        var token = await GetTokenAsync(username, password);
        if (!string.IsNullOrEmpty(token?.Token))
        {
            await CallGraphApi(token.Token);
        }
    }

    private async Task<ResponseToken?> GetTokenAsync(string? username, string? password)
    {
        var bodyBuilder = new StringBuilder();
        bodyBuilder.Append($"client_id={ClientID}");
        bodyBuilder.Append($"&client_secret={ClientSecret}");
        bodyBuilder.Append($"&scope={string.Join(' ', Scopes)}");
        bodyBuilder.Append($"&username={username}");
        bodyBuilder.Append($"&password={password}");
        bodyBuilder.Append($"&grant_type=password");

        var content = new StringContent(bodyBuilder.ToString());
        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

        var response = await Client.PostAsync($"{TenantUrl}token", content);
        return await ExtractTokenAsync(response);
    }
    private void LoginScreen(out string? username, out string? password)
    {
        Console.Write("Login (MUST be a DOMAIN user): ");
        username = Console.ReadLine();
        Console.Write("Password: ");
        Console.ForegroundColor = Console.BackgroundColor;
        password = Console.ReadLine();
        Console.ResetColor();
    }
}
