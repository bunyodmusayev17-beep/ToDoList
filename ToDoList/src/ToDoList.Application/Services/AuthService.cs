using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Converters;
using ToDoList.Application.Dtos;
using ToDoList.Application.Exceptions;
using ToDoList.Application.Settings;
using ToDoList.Domain.Entities;

namespace ToDoList.Application.Services;

public class AuthService : IAuthService
{
    private readonly IBaseRepository<User> _userRepository;
    private readonly IBaseRepository<RefreshToken> _refreshTokenRepository;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IBaseRepository<User> userRepository,
        IPasswordHasherService passwordHasherService,
        ITokenService tokenService,
        IBaseRepository<RefreshToken> refreshTokenRepository,
        JwtSettings jwtSettings,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _passwordHasherService = passwordHasherService;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtSettings = jwtSettings;
        _logger = logger;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
    {
        var users = _userRepository.GetAllQuery();

        var user = await users.FirstOrDefaultAsync(u =>
                    u.UserName == loginDto.UserNameOrEmail
                    || u.Email == loginDto.UserNameOrEmail);

        if (user == null)
        {
            throw new UnauthorizedException("Invalid username or email.");
        }

        var isPasswordValid = _passwordHasherService.Verify(loginDto.Password, user.Password, user.Salt);

        if (!isPasswordValid)
        {
            _logger.LogWarning("Failed login attempt for {Identifier}.", loginDto.UserNameOrEmail);
            throw new UnauthorizedException("Invalid password.");
        }

        var loginResponseDto = await GenerateLoginResponseAsync(user);

        _logger.LogInformation("User {UserId} logged in.", user.UserId);

        return loginResponseDto;
    }

    public async Task<LoginResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto)
    {
        var storedToken = await _refreshTokenRepository.GetAllQuery()
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == refreshTokenRequestDto.RefreshToken);

        if (storedToken == null || !storedToken.IsActive)
        {
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        var loginResponseDto = await GenerateLoginResponseAsync(storedToken.User);

        // Rotate: revoke the old refresh token and link it to the newly issued one.
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.ReplacedByToken = loginResponseDto.RefreshToken;
        _refreshTokenRepository.Update(storedToken);
        await _refreshTokenRepository.SaveChangesAsync();

        _logger.LogInformation("Refresh token rotated for user {UserId}.", storedToken.UserId);

        return loginResponseDto;
    }

    public async Task LogoutAsync(RefreshTokenRequestDto refreshTokenRequestDto)
    {
        var storedToken = await _refreshTokenRepository.GetAllQuery()
            .FirstOrDefaultAsync(x => x.Token == refreshTokenRequestDto.RefreshToken);

        if (storedToken == null)
        {
            throw new NotFoundException("Refresh token not found.");
        }

        if (storedToken.IsActive)
        {
            storedToken.RevokedAt = DateTime.UtcNow;
            _refreshTokenRepository.Update(storedToken);
            await _refreshTokenRepository.SaveChangesAsync();
            _logger.LogInformation("User {UserId} logged out.", storedToken.UserId);
        }
    }

    public async Task<int> PurgeExpiredRefreshTokensAsync()
    {
        var now = DateTime.UtcNow;

        var staleTokens = await _refreshTokenRepository.GetAllQuery()
            .Where(x => x.RevokedAt != null || x.ExpiresAt <= now)
            .ToListAsync();

        foreach (var token in staleTokens)
        {
            _refreshTokenRepository.Delete(token);
        }

        if (staleTokens.Count > 0)
        {
            await _refreshTokenRepository.SaveChangesAsync();
            _logger.LogInformation("Purged {Count} expired/revoked refresh tokens.", staleTokens.Count);
        }

        return staleTokens.Count;
    }

    private async Task<LoginResponseDto> GenerateLoginResponseAsync(User user)
    {
        var userGetDto = new UserGetDto()
        {
            UserId = user.UserId,
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailConfirmed = user.EmailConfirmed,
            CreatedAt = user.CreatedAt
        };

        var accessToken = _tokenService.GetToken(userGetDto);
        var refreshTokenValue = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken()
        {
            Token = refreshTokenValue,
            UserId = user.UserId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenLifetimeDays),
        };

        await _refreshTokenRepository.AddAsync(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return new LoginResponseDto()
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            TokenType = "Bearer",
            Expires = _jwtSettings.Lifetime,
        };
    }

    public async Task<long> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _userRepository.GetAllQuery()
            .FirstOrDefaultAsync(u => u.UserName == registerDto.UserName || u.Email == registerDto.Email);

        if (existingUser != null)
        {
            throw new EmailAlreadyExistsException("A user with the same username or email already exists.");
        }

        var hashedPassword = _passwordHasherService.Hasher(registerDto.Password);
        registerDto.Password = hashedPassword.Hash;

        var newUser = registerDto.ToEntity();
        newUser.CreatedAt = DateTime.UtcNow;
        newUser.UpdatedAt = DateTime.UtcNow;
        newUser.Salt = hashedPassword.Salt;
        newUser.Role = UserRole.User;
        newUser.EmailConfirmed = false;

        await _userRepository.AddAsync(newUser);
        await _userRepository.SaveChangesAsync();

        _logger.LogInformation("New user registered: {UserId}.", newUser.UserId);

        return newUser.UserId;
    }
}
