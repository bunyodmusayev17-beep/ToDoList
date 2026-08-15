using ToDoList.Domain.Entities;

namespace ToDoList.Application.Abstractions;

public interface ICurrentUserService
{
    long? UserId { get; }
    string? UserName { get; }
    string? FirstName { get; }
    string? LastName { get; }
    string? Email { get; }
    UserRole? Role { get; }
}
