using ToDoList.Application.Abstractions;
using ToDoList.Domain.Entities;

namespace ToDoList.Api.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor HttpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        HttpContextAccessor = httpContextAccessor;
    }
    public string? UserName => GetUserName();

    public string? FirstName => GetFirstName();

    public string? LastName => GetLastName();

    public string? Email => GetEmail();

    public UserRole? Role => GetRole();

    public long? UserId => GetUserId();

    private long? GetUserId()
    {
        var userIdClaim = HttpContextAccessor.HttpContext?.User?.FindFirst("UserId");
        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }
        return null;
    }

    private string? GetFirstName()
    {
        var firstNameClaim = HttpContextAccessor.HttpContext?.User?.FindFirst("FirstName");
        if (firstNameClaim != null)
        {
            return firstNameClaim.Value;
        }
        return null;
    }

    private string? GetLastName()
    {
        var lastNameClaim = HttpContextAccessor.HttpContext?.User?.FindFirst("LastName");
        if (lastNameClaim != null)
        {
            return lastNameClaim.Value;
        }
        return null;
    }

    private string? GetEmail()
    {
        var emailClaim = HttpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.Email);
        if (emailClaim != null)
        {
            return emailClaim.Value;
        }
        return null;
    }

    private UserRole? GetRole()
    {
        var roleClaim = HttpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.Role);
        if (roleClaim != null && Enum.TryParse<UserRole>(roleClaim.Value, out var role))
        {
            return role;
        }
        return null;
    }

    private string? GetUserName()
    {
        var userNameClaim = HttpContextAccessor.HttpContext?.User?.FindFirst("UserName");
        if (userNameClaim != null)
        {
            return userNameClaim.Value;
        }
        return null;
    }
}
