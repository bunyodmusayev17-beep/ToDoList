using ToDoList.Domain.Entities;

namespace ToDoList.Application.Dtos;

public class ToDoItemCreateDto
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;
    public DateTime? DueDate { get; set; }
    public DateTime? ReminderAt { get; set; }
}
