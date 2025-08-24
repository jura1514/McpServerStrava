using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StravaMcpServer.Configuration;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.Configure<StravaConfiguration>(
    builder.Configuration.GetSection(StravaConfiguration.SectionName));

builder.Services.AddLogging();
builder.Services.AddHttpClient("StravaClient", (services, client) =>
{
    var stravaConfig = services.GetRequiredService<IOptions<StravaConfiguration>>().Value;
    client.BaseAddress = stravaConfig?.BaseUrl;
});

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly()
    .WithPromptsFromAssembly()
    .WithResourcesFromAssembly();

await builder.Build().RunAsync();
