using Interfaces;
using Microsoft.Extensions.Logging.AzureAppServices;
using Repository.InMemory;
using Serializers;

namespace DataService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.AddConsole();
        builder.Services.Configure<AzureBlobLoggerOptions>(options => { 
            options.BlobName = "dataservice_log.txt";
        });
        builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

        builder.Services.AddCors(opts => {
            opts.AddPolicy("all", cp => {
                cp.AllowAnyOrigin();
            });
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddControllers()
        .AddNewtonsoftJson(options => {
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            options.SerializerSettings.MaxDepth = 2;
            options.SerializerSettings.ContractResolver = new SimplePropertyContractResolver();
        });
        //builder.Services.AddLogging(conf =>
        //{
        //    conf.AddApplicationInsights(options =>
        //    {
        //        options..ConnectionString = "";// = builder.Configuration["APPINSIGHTS_CONNECTIONSTRING"];
        //    });
        //});
        var app = builder.Build();


        app.UseCors();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
