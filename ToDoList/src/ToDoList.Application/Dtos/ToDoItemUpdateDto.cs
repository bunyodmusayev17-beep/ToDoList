using ToDoList.Domain.Entities;

namespace ToDoList.Application.Dtos;

public class ToDoItemUpdateDto
{
    public string Title { get; set; }
    public string? Description { get; set; }
    public PriorityLevel Priority { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? ReminderAt { get; set; }
}
