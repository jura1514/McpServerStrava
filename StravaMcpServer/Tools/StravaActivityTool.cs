using System.ComponentModel;
using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;

namespace StravaMcpServer.Tools;

[McpServerToolType]
public static class StravaActivityTool
{
    [McpServerTool(Name = "ProvideFeedbackOnLastRunningActivity")]
    [Description("Provides feedback on the last running activity of the user.")]
    public static async Task<string> ProvideFeedbackOnLastRunningActivity(IMcpServer mcpServer, HttpClient httpClient,
        CancellationToken cancellationToken = default)
    {
        var accessToken = Environment.GetEnvironmentVariable("STRAVA_ACCESS_TOKEN");

        var activity =
            await httpClient.GetStringAsync(
                $"http://localhost:5033/api/strava/activities/1?accessToken={accessToken}&page=1&perPage=1",
                cancellationToken);

        ChatMessage[] messages =
        [
            new(ChatRole.User,
                "Provide constructive feedback on my last running activity. Tell me where i excel and where I can improve."),
            new(ChatRole.User, activity)
        ];

        ChatOptions options = new()
        {
            Instructions =
                "You are a professional running coach with a vast experience in training professional athletes."
        };

        var response = await mcpServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"Feedback: {response}";
    }
}