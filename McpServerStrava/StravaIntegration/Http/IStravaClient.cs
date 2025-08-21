namespace StravaIntegration.Http;

public interface IStravaClient
{
    Task<TResponse?> GetAsync<TResponse>(string endpoint, string? accessToken = null);
    Task<TResponse?> PostAsync<TResponse>(string endpoint, object? data = null, string? accessToken = null);
    Task<TResponse?> PostFormAsync<TResponse>(string endpoint, Dictionary<string, string> formData);
}