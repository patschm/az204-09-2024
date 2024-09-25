using Azure.Identity;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("CON_APPCONFIG");
var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

// Add services to the container.

builder.Configuration.AddAzureAppConfiguration(options =>
{
    options.Connect(connectionString)
        .Select(KeyFilter.Any, "Production")
        .UseFeatureFlags(opts =>
        {
            opts.Select(KeyFilter.Any, "Production");
            opts.CacheExpirationInterval = TimeSpan.FromSeconds(10);
        });
    options.ConfigureKeyVault(conf =>
    {
        var token = new DefaultAzureCredential();
        conf.SetCredential(token);
    });
});
builder.Services.AddFeatureManagement();
builder.Services.AddAzureAppConfiguration();
builder.Services.AddFeatureManagement();
builder.Services.AddControllersWithViews();
//builder.Services.AddKeyVault()

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Use Azure App Configuration middleware for dynamic configuration refresh.
app.UseAzureAppConfiguration();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Calculator}/{action=Index}");

app.Run();
