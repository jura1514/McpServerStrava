using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol.Client;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

var openAiClient = new AzureOpenAIClient(new Uri("URL_GOES_HERE"),
        new AzureKeyCredential("KEY_GOES_HERE"))
    .GetChatClient("gpt-4o-mini").AsIChatClient();

var client =
    new ChatClientBuilder(openAiClient)
        .UseFunctionInvocation()
        .Build();

var clientTransport = new StdioClientTransport(new StdioClientTransportOptions
{
    Name = "strava-mcp-server",
    Command = "dotnet",
    Arguments = ["run", "--project", "../StravaMcpServer/StravaMcpServer.csproj", "--no-build"]
});

await using var mcpClient = await McpClientFactory.CreateAsync(clientTransport);
var tools = await mcpClient.ListToolsAsync();

foreach (var tool in tools)
{
    Console.WriteLine($"Connected to server with tools: {tool.Name}");
}

List<ChatMessage> messages = [];
while (true)
{
    Console.Write("Prompt: ");
    messages.Add(new ChatMessage(ChatRole.User, Console.ReadLine()));

    List<ChatResponseUpdate> updates = [];
    await foreach (var update in client
                       .GetStreamingResponseAsync(messages, new ChatOptions { Tools = [.. tools] }))
    {
        Console.Write(update);
        updates.Add(update);
    }

    Console.WriteLine();

    messages.AddMessages(updates);
}