using System.Text.Json.Serialization;

namespace ToDoList.BlazorUI.Models;

/// <summary>Outcome of an API call, including a friendly error message and field errors.</summary>
public class ApiResult
{
    public bool Success { get; init; }
    public int StatusCode { get; init; }
    public string? ErrorMessage { get; init; }
    public IReadOnlyDictionary<string, string[]>? ValidationErrors { get; init; }

    public static ApiResult Ok(int statusCode = 200) => new() { Success = true, StatusCode = statusCode };

    public static ApiResult Fail(int statusCode, string? message, IReadOnlyDictionary<string, string[]>? errors = null)
        => new() { Success = false, StatusCode = statusCode, ErrorMessage = message, ValidationErrors = errors };
}

public class ApiResult<T> : ApiResult
{
    public T? Data { get; init; }

    public static ApiResult<T> Ok(T data, int statusCode = 200)
        => new() { Success = true, StatusCode = statusCode, Data = data };

    public static new ApiResult<T> Fail(int statusCode, string? message, IReadOnlyDictionary<string, string[]>? errors = null)
        => new() { Success = false, StatusCode = statusCode, ErrorMessage = message, ValidationErrors = errors };
}

/// <summary>Mirrors ASP.NET Core ProblemDetails / ValidationProblemDetails responses.</summary>
public class ProblemDetailsResponse
{
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    [JsonPropertyName("status")]
    public int? Status { get; set; }

    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }
}
