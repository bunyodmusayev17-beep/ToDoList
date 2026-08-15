using ToDoList.Application.Dtos;
using ToDoList.Domain.Entities;

namespace ToDoList.Application.Converters;

public static class ToDoItemConverter
{
    public static ToDoItem ToEntity(this ToDoItemCreateDto dto, long userId)
    {
        var now = DateTime.UtcNow;
        return new ToDoItem
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            ReminderAt = dto.ReminderAt,
            UserId = userId,
            IsCompleted = false,
            IsDeleted = false,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public static void ApplyTo(this ToDoItemUpdateDto dto, ToDoItem entity)
    {
        entity.Title = dto.Title;
        entity.Description = dto.Description;
        entity.Priority = dto.Priority;
        entity.DueDate = dto.DueDate;
        entity.ReminderAt = dto.ReminderAt;

        // Keep CompletedAt in sync with the completion state.
        if (dto.IsCompleted && !entity.IsCompleted)
        {
            entity.CompletedAt = DateTime.UtcNow;
        }
        else if (!dto.IsCompleted && entity.IsCompleted)
        {
            entity.CompletedAt = null;
        }

        entity.IsCompleted = dto.IsCompleted;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    public static ToDoItemGetDto ToGetDto(this ToDoItem entity)
    {
        return new ToDoItemGetDto
        {
            ToDoItemId = entity.ToDoItemId,
            Title = entity.Title,
            Description = entity.Description,
            IsCompleted = entity.IsCompleted,
            IsDeleted = entity.IsDeleted,
            Priority = entity.Priority,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            DueDate = entity.DueDate,
            CompletedAt = entity.CompletedAt,
            DeletedAt = entity.DeletedAt,
            ReminderAt = entity.ReminderAt
        };
    }
}
