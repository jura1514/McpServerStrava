using System.Net.Http.Headers;

namespace StravaIntegration.Http;

public class StravaClient : IStravaClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<StravaClient> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public StravaClient(HttpClient httpClient, ILogger<StravaClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<TResponse?> GetAsync<TResponse>(string endpoint, string? accessToken = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        AddAuthorizationHeader(request, accessToken);

        return await SendRequestAsync<TResponse>(request, endpoint);
    }

    public async Task<TResponse?> PostAsync<TResponse>(string endpoint, object? data = null, string? accessToken = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint);

        if (data != null)
        {
            var jsonData = JsonSerializer.Serialize(data, JsonOptions);
            request.Content = new StringContent(jsonData, System.Text.Encoding.UTF8, "application/json");
        }

        AddAuthorizationHeader(request, accessToken);

        return await SendRequestAsync<TResponse>(request, endpoint);
    }

    public async Task<TResponse?> PostFormAsync<TResponse>(string endpoint, Dictionary<string, string> formData)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, endpoint)
        {
            Content = new FormUrlEncodedContent(formData)
        };

        return await SendRequestAsync<TResponse>(request, endpoint);
    }

    private static void AddAuthorizationHeader(HttpRequestMessage request, string? accessToken)
    {
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }

    private async Task<TResponse?> SendRequestAsync<TResponse>(HttpRequestMessage request, string endpoint)
    {
        try
        {
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                await LogErrorAsync(request.Method.Method, endpoint, response);
                return default;
            }

            return await DeserializeResponseAsync<TResponse>(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during {Method} request to {Endpoint}",
                request.Method.Method, endpoint);
            return default;
        }
    }

    private async Task LogErrorAsync(string method, string endpoint, HttpResponseMessage response)
    {
        var errorContent = await response.Content.ReadAsStringAsync();
        _logger.LogError("{Method} request failed for {Endpoint}. Status: {StatusCode}, Content: {Content}",
            method, endpoint, response.StatusCode, errorContent);
    }

    private static async Task<TResponse?> DeserializeResponseAsync<TResponse>(HttpResponseMessage response)
    {
        var jsonContent = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(jsonContent, JsonOptions);
    }
}