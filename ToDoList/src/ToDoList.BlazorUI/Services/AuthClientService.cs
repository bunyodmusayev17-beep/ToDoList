using ToDoList.BlazorUI.Auth;
using ToDoList.BlazorUI.Models;

namespace ToDoList.BlazorUI.Services;

public class AuthClientService
{
    private readonly ApiClient _api;
    private readonly ITokenStore _tokenStore;
    private readonly JwtAuthenticationStateProvider _authProvider;

    public AuthClientService(
        ApiClient api,
        ITokenStore tokenStore,
        JwtAuthenticationStateProvider authProvider)
    {
        _api = api;
        _tokenStore = tokenStore;
        _authProvider = authProvider;
    }

    public async Task<ApiResult> LoginAsync(LoginRequest request)
    {
        var result = await _api.PostAsync<LoginResponse>("api/v1/auth/login", request);
        if (!result.Success || result.Data is null)
        {
            return ApiResult.Fail(result.StatusCode, result.ErrorMessage, result.ValidationErrors);
        }

        await _tokenStore.SaveTokensAsync(result.Data.AccessToken, result.Data.RefreshToken);
        _authProvider.NotifyUserAuthentication(result.Data.AccessToken);
        return ApiResult.Ok(result.StatusCode);
    }

    public async Task<ApiResult> RegisterAsync(RegisterRequest request)
        => await _api.PostAsync("api/v1/auth/register", request);

    public async Task<ApiResult<PurgeResult>> PurgeTokensAsync()
        => await _api.PostAsync<PurgeResult>("api/v1/auth/purge-tokens");

    public async Task LogoutAsync()
    {
        var refreshToken = await _tokenStore.GetRefreshTokenAsync();
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            // Best-effort revoke on the server; ignore failures.
            await _api.PostAsync("api/v1/auth/logout", new RefreshTokenRequest { RefreshToken = refreshToken });
        }

        await _tokenStore.ClearAsync();
        _authProvider.NotifyUserLogout();
    }
}
