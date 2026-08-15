using Blazored.LocalStorage;

namespace ToDoList.BlazorUI.Auth;

public interface ITokenStore
{
    Task<string?> GetAccessTokenAsync();
    Task<string?> GetRefreshTokenAsync();
    Task SaveTokensAsync(string accessToken, string refreshToken);
    Task ClearAsync();
}

public class TokenStore : ITokenStore
{
    private const string AccessTokenKey = "todolist_access_token";
    private const string RefreshTokenKey = "todolist_refresh_token";

    private readonly ILocalStorageService _localStorage;

    public TokenStore(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public async Task<string?> GetAccessTokenAsync()
        => await _localStorage.GetItemAsync<string?>(AccessTokenKey);

    public async Task<string?> GetRefreshTokenAsync()
        => await _localStorage.GetItemAsync<string?>(RefreshTokenKey);

    public async Task SaveTokensAsync(string accessToken, string refreshToken)
    {
        await _localStorage.SetItemAsync(AccessTokenKey, accessToken);
        await _localStorage.SetItemAsync(RefreshTokenKey, refreshToken);
    }

    public async Task ClearAsync()
    {
        await _localStorage.RemoveItemAsync(AccessTokenKey);
        await _localStorage.RemoveItemAsync(RefreshTokenKey);
    }
}
