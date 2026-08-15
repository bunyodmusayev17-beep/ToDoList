using System.Net.Http.Json;
using System.Text.Json;
using ToDoList.BlazorUI.Models;

namespace ToDoList.BlazorUI.Services;

/// <summary>Thin wrapper over HttpClient that returns ApiResult and maps ProblemDetails errors.</summary>
public class ApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;

    public ApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<ApiResult<T>> GetAsync<T>(string url)
        => await SendAsync<T>(() => _http.GetAsync(url));

    public async Task<ApiResult<TResponse>> PostAsync<TResponse>(string url, object body)
        => await SendAsync<TResponse>(() => _http.PostAsJsonAsync(url, body, JsonOptions));

    public async Task<ApiResult<TResponse>> PostAsync<TResponse>(string url)
        => await SendAsync<TResponse>(() => _http.PostAsync(url, null));

    public async Task<ApiResult> PostAsync(string url, object? body = null)
        => await SendAsync(() => body is null
            ? _http.PostAsync(url, null)
            : _http.PostAsJsonAsync(url, body, JsonOptions));

    public async Task<ApiResult<TResponse>> PutAsync<TResponse>(string url, object body)
        => await SendAsync<TResponse>(() => _http.PutAsJsonAsync(url, body, JsonOptions));

    public async Task<ApiResult<TResponse>> PatchAsync<TResponse>(string url)
        => await SendAsync<TResponse>(() => _http.PatchAsync(url, null));

    public async Task<ApiResult> DeleteAsync(string url)
        => await SendAsync(() => _http.DeleteAsync(url));

    private async Task<ApiResult<T>> SendAsync<T>(Func<Task<HttpResponseMessage>> send)
    {
        try
        {
            var response = await send();
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
                return ApiResult<T>.Ok(data!, (int)response.StatusCode);
            }

            var (message, errors) = await ReadProblemAsync(response);
            return ApiResult<T>.Fail((int)response.StatusCode, message, errors);
        }
        catch (Exception ex)
        {
            return ApiResult<T>.Fail(0, FriendlyNetworkError(ex));
        }
    }

    private async Task<ApiResult> SendAsync(Func<Task<HttpResponseMessage>> send)
    {
        try
        {
            var response = await send();
            if (response.IsSuccessStatusCode)
            {
                return ApiResult.Ok((int)response.StatusCode);
            }

            var (message, errors) = await ReadProblemAsync(response);
            return ApiResult.Fail((int)response.StatusCode, message, errors);
        }
        catch (Exception ex)
        {
            return ApiResult.Fail(0, FriendlyNetworkError(ex));
        }
    }

    private static async Task<(string? message, IReadOnlyDictionary<string, string[]>? errors)> ReadProblemAsync(
        HttpResponseMessage response)
    {
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemDetailsResponse>(JsonOptions);
            if (problem is not null)
            {
                var message = problem.Detail ?? problem.Title;
                return (message, problem.Errors);
            }
        }
        catch
        {
            // Body was not ProblemDetails JSON — fall through to a generic message.
        }

        return ((int)response.StatusCode switch
        {
            401 => "Your session has expired. Please sign in again.",
            403 => "You do not have permission to perform this action.",
            404 => "The requested resource was not found.",
            429 => "Too many requests. Please wait a moment and try again.",
            _ => "Something went wrong. Please try again."
        }, null);
    }

    private static string FriendlyNetworkError(Exception ex)
        => $"Could not reach the server. Check that the API is running. ({ex.Message})";
}
