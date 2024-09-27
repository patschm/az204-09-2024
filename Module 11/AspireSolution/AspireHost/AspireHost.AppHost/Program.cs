var builder = DistributedApplication.CreateBuilder(args);
var weather = builder.AddProject("weather", @"..\Backend.Weather\Backend.Weather.csproj");

builder.Build().Run();
