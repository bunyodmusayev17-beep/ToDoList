using ToDoList.Domain.Entities;

namespace ToDoList.Application.Dtos;

public class UserGetDto
{
    public long UserId { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public UserRole Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<ToDoItemGetDto>? ToDoItemGetDto { get; set; }
}
