using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ToDoList.BlazorUI.Models;

namespace ToDoList.BlazorUI.Auth;

/// <summary>
/// Attaches the JWT access token to outgoing requests and, on a 401, transparently
/// refreshes the token once and retries. If refresh fails, it clears the session.
/// </summary>
public class AuthHeaderHandler : DelegatingHandler
{
    private static readonly SemaphoreSlim RefreshLock = new(1, 1);

    private readonly ITokenStore _tokenStore;
    private readonly JwtAuthenticationStateProvider _authProvider;
    private readonly HttpClient _refreshClient;

    public AuthHeaderHandler(
        ITokenStore tokenStore,
        JwtAuthenticationStateProvider authProvider,
        IConfiguration configuration)
    {
        _tokenStore = tokenStore;
        _authProvider = authProvider;

        var baseUrl = configuration["ApiBaseUrl"] ?? "https://localhost:7050/";
        _refreshClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var isAuthEndpoint = request.RequestUri?.AbsolutePath.Contains("/auth/", StringComparison.OrdinalIgnoreCase) ?? false;

        // Keep a copy so the request can be replayed after a token refresh.
        var retryClone = isAuthEndpoint ? null : await CloneRequestAsync(request);

        var accessToken = await _tokenStore.GetAccessTokenAsync();
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized || retryClone is null)
        {
            return response;
        }

        var newAccessToken = await TryRefreshAsync(accessToken, cancellationToken);
        if (string.IsNullOrWhiteSpace(newAccessToken))
        {
            await _tokenStore.ClearAsync();
            _authProvider.NotifyUserLogout();
            return response;
        }

        response.Dispose();
        retryClone.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newAccessToken);
        return await base.SendAsync(retryClone, cancellationToken);
    }

    private async Task<string?> TryRefreshAsync(string? usedAccessToken, CancellationToken cancellationToken)
    {
        await RefreshLock.WaitAsync(cancellationToken);
        try
        {
            // Another request may have already refreshed the token while we waited.
            var current = await _tokenStore.GetAccessTokenAsync();
            if (!string.IsNullOrWhiteSpace(current) && current != usedAccessToken)
            {
                return current;
            }

            var refreshToken = await _tokenStore.GetRefreshTokenAsync();
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return null;
            }

            var response = await _refreshClient.PostAsJsonAsync(
                "api/v1/auth/refresh-token",
                new RefreshTokenRequest { RefreshToken = refreshToken },
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var login = await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken);
            if (login is null || string.IsNullOrWhiteSpace(login.AccessToken))
            {
                return null;
            }

            await _tokenStore.SaveTokensAsync(login.AccessToken, login.RefreshToken);
            _authProvider.NotifyUserAuthentication(login.AccessToken);
            return login.AccessToken;
        }
        catch
        {
            return null;
        }
        finally
        {
            RefreshLock.Release();
        }
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);

        if (request.Content is not null)
        {
            var bytes = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(bytes);
            foreach (var header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }
}
