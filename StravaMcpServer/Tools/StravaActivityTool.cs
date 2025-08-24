using System.ComponentModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;
using StravaMcpServer.Configuration;

namespace StravaMcpServer.Tools;

[McpServerToolType]
public static class StravaActivityTool
{
    [McpServerTool(Name = "AnalyzeRunningActivity")]
    [Description(
        "Provides professional coaching analysis of your latest running activity with performance assessment, improvement recommendations, strengths identification, and race predictions for 10K, half marathon, and marathon with time estimates and readiness levels.")]
    public static async Task<string> AnalyzeRunningActivity(IMcpServer mcpServer,
        IHttpClientFactory httpClientFactory,
        IOptions<StravaConfiguration> stravaConfig,
        [Description("Access token for strava API.")]
        string? stravaApiAccessToken = null,
        CancellationToken cancellationToken = default)
    {
        var accessToken = stravaApiAccessToken ?? stravaConfig.Value.AccessToken;

        var httpClient = httpClientFactory.CreateClient("StravaClient");
        var activityResponse =
            await httpClient.GetAsync($"/api/strava/activities/1?accessToken={accessToken}&page=1&perPage=30",
                cancellationToken);

        if (!activityResponse.IsSuccessStatusCode)
        {
            return $"Error fetching activity data: {activityResponse.StatusCode}";
        }

        var activity = await activityResponse.Content.ReadAsStringAsync(cancellationToken);

        ChatMessage[] messages =
        [
            new(ChatRole.User,
                "Analyze my latest running activity and provide comprehensive professional coaching feedback. I want a detailed assessment covering:\n\n" +
                "1. OVERALL PERFORMANCE ANALYSIS:\n" +
                "   - Pace consistency throughout the run\n" +
                "   - Heart rate zones and efficiency\n" +
                "   - Cadence and stride analysis\n" +
                "   - Energy management and pacing strategy\n\n" +
                "2. AREAS FOR IMPROVEMENT:\n" +
                "   - Specific technical aspects to work on\n" +
                "   - Training recommendations for weaknesses\n" +
                "   - Pacing and strategy adjustments\n" +
                "   - Recovery and preparation suggestions\n\n" +
                "3. POSITIVE AREAS & STRENGTHS:\n" +
                "   - What I'm doing well and should continue\n" +
                "   - Strong performance metrics to build upon\n" +
                "   - Progress indicators from previous activities\n\n" +
                "4. RACE PREDICTIONS & READINESS:\n" +
                "   - Estimated 10K time based on current fitness\n" +
                "   - Estimated half marathon time and target pace\n" +
                "   - Current half marathon readiness level (1-10 scale)\n" +
                "   - Marathon readiness assessment with timeline\n" +
                "   - Recommended race pace for each distance\n" +
                "   - Training plan suggestions for race preparation\n\n" +
                "Provide specific, actionable advice with numerical targets where possible."),
            new(ChatRole.User, activity)
        ];

        ChatOptions options = new()
        {
            Instructions =
                "You are an elite professional running coach with 20+ years of experience training Olympic athletes, marathon world record holders, and recreational runners. " +
                "You have deep expertise in exercise physiology, biomechanics, training periodization, and race strategy. " +
                "Analyze running data with the precision of a sports scientist and provide coaching feedback with the wisdom of a master coach.\n\n" +
                "ANALYSIS APPROACH:\n" +
                "- Examine all available metrics (pace, heart rate, cadence, elevation, splits)\n" +
                "- Consider physiological indicators and training stress\n" +
                "- Assess pacing strategy and energy distribution\n" +
                "- Evaluate technique indicators from cadence and pace data\n" +
                "- Compare performance to established running standards\n\n" +
                "FEEDBACK STYLE:\n" +
                "- Be specific and data-driven in your analysis\n" +
                "- Provide actionable recommendations with clear next steps\n" +
                "- Use professional coaching terminology appropriately\n" +
                "- Include specific time goals and pace targets\n" +
                "- Balance honest assessment with motivational guidance\n" +
                "- Reference established training principles and methodologies\n\n" +
                "RACE PREDICTIONS:\n" +
                "- Use scientifically-backed formulas (Riegel, Daniels, McMillan) for time predictions\n" +
                "- Consider current fitness level, training volume, and recent performance trends\n" +
                "- Provide realistic timelines for achieving race readiness\n" +
                "- Factor in typical improvement rates and training adaptations"
        };

        var response = await mcpServer.AsSamplingChatClient().GetResponseAsync(messages, options, cancellationToken);
        return $"Feedback:\n{response}";
    }
}