using ToDoList.Application.Settings;

namespace ToDoList.Api.Configurations;

public static class JwtSettingConfiguration
{
    public static void ConfigureJwt(this WebApplicationBuilder builder)
    {
        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];
        var secretKey = builder.Configuration["Jwt:SecurityKey"];
        var lifetime = builder.Configuration["Jwt:Lifetime"];
        var refreshTokenLifetimeDays = builder.Configuration["Jwt:RefreshTokenLifetimeDays"];

        var jwtSettings = new JwtSettings
        {
            Issuer = issuer,
            Audience = audience,
            SecretKey = secretKey,
            Lifetime = int.Parse(lifetime),
            RefreshTokenLifetimeDays = int.Parse(refreshTokenLifetimeDays)
        };

        builder.Services.AddSingleton(jwtSettings);
    }
}
