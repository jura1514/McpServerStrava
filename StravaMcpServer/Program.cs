using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StravaMcpServer.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<StravaConfiguration>(
    builder.Configuration.GetSection(StravaConfiguration.SectionName));

builder.Services.AddLogging();
builder.Services.AddHttpClient("StravaClient", client =>
{
    var stravaConfig = builder.Configuration.GetSection(StravaConfiguration.SectionName).Get<StravaConfiguration>();
    client.BaseAddress = stravaConfig?.BaseUrl;
});

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly()
    .WithPromptsFromAssembly()
    .WithResourcesFromAssembly();

await builder.Build().RunAsync();
