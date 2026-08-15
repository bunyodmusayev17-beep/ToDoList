using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

namespace ToDoList.BlazorUI.Auth;

public class JwtAuthenticationStateProvider : AuthenticationStateProvider
{
    private static readonly AuthenticationState Anonymous =
        new(new ClaimsPrincipal(new ClaimsIdentity()));

    private readonly ITokenStore _tokenStore;

    public JwtAuthenticationStateProvider(ITokenStore tokenStore)
    {
        _tokenStore = tokenStore;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _tokenStore.GetAccessTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            return Anonymous;
        }

        var claims = ParseClaimsFromJwt(token).ToList();
        if (claims.Count == 0)
        {
            return Anonymous;
        }

        // Expiry is intentionally NOT enforced here — the HTTP handler transparently
        // refreshes the access token on a 401, keeping the user signed in.
        var identity = new ClaimsIdentity(claims, "jwt", "UserName", ClaimTypes.Role);
        return new AuthenticationState(new ClaimsPrincipal(identity));
    }

    public void NotifyUserAuthentication(string accessToken)
    {
        var claims = ParseClaimsFromJwt(accessToken);
        var identity = new ClaimsIdentity(claims, "jwt", "UserName", ClaimTypes.Role);
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(Anonymous));
    }

    private static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();

        var parts = jwt.Split('.');
        if (parts.Length < 2)
        {
            return claims;
        }

        byte[] payloadBytes;
        try
        {
            payloadBytes = ParseBase64WithoutPadding(parts[1]);
        }
        catch
        {
            return claims;
        }

        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(payloadBytes);
        if (keyValuePairs is null)
        {
            return claims;
        }

        foreach (var kvp in keyValuePairs)
        {
            var key = kvp.Key;
            var element = kvp.Value;

            if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray())
                {
                    AddClaim(claims, key, item.ToString());
                }
            }
            else
            {
                AddClaim(claims, key, element.ToString());
            }
        }

        return claims;
    }

    private static void AddClaim(List<Claim> claims, string key, string value)
    {
        claims.Add(new Claim(key, value));

        // Normalise role / email so AuthorizeView(Roles=...) and Identity checks work
        // regardless of whether the token uses short names or the full claim-type URIs.
        if (key is "role" || key.EndsWith("/role", StringComparison.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(ClaimTypes.Role, value));
        }
        else if (key is "email" || key.EndsWith("/emailaddress", StringComparison.OrdinalIgnoreCase))
        {
            claims.Add(new Claim(ClaimTypes.Email, value));
        }
    }

    private static byte[] ParseBase64WithoutPadding(string base64)
    {
        base64 = base64.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        return Convert.FromBase64String(base64);
    }
}
