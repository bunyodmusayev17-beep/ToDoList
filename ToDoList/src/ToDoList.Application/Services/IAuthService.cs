using ToDoList.Application.Dtos;

namespace ToDoList.Application.Services;

public interface IAuthService
{
    Task<long> RegisterAsync(RegisterDto registerDto);
    Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
    Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto);
    Task LogoutAsync(RefreshTokenRequestDto refreshTokenRequestDto);
    Task<int> PurgeExpiredRefreshTokensAsync();
}
