using Microsoft.EntityFrameworkCore;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Converters;
using ToDoList.Application.Dtos;
using ToDoList.Application.Exceptions;
using ToDoList.Domain.Entities;

namespace ToDoList.Application.Services;

public class ToDoItemService : IToDoItemService
{
    private readonly IBaseRepository<ToDoItem> _repository;
    private readonly ICurrentUserService _currentUserService;

    public ToDoItemService(
        IBaseRepository<ToDoItem> repository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<ToDoItemGetDto> CreateAsync(ToDoItemCreateDto dto)
    {
        var userId = GetCurrentUserId();

        var entity = dto.ToEntity(userId);

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return entity.ToGetDto();
    }

    public async Task<ToDoItemGetDto> GetByIdAsync(long id)
    {
        var entity = await GetOwnedItemAsync(id);
        return entity.ToGetDto();
    }

    public async Task<PagedResult<ToDoItemGetDto>> GetAllAsync(ToDoItemQueryParams query)
    {
        var userId = GetCurrentUserId();

        var queryable = _repository.GetAllQuery()
            .Where(x => x.UserId == userId);

        if (query.IsCompleted.HasValue)
        {
            queryable = queryable.Where(x => x.IsCompleted == query.IsCompleted.Value);
        }

        if (query.Priority.HasValue)
        {
            queryable = queryable.Where(x => x.Priority == query.Priority.Value);
        }

        if (query.DueFrom.HasValue)
        {
            queryable = queryable.Where(x => x.DueDate >= query.DueFrom.Value);
        }

        if (query.DueTo.HasValue)
        {
            queryable = queryable.Where(x => x.DueDate <= query.DueTo.Value);
        }

        var totalCount = queryable.Count();

        queryable = ApplySorting(queryable, query);

        var items = queryable
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToList();

        var dtos = items.Select(x => x.ToGetDto()).ToList();

        return new PagedResult<ToDoItemGetDto>(dtos, totalCount, query.Page, query.PageSize);
    }

    public async Task<ToDoItemGetDto> UpdateAsync(long id, ToDoItemUpdateDto dto)
    {
        var entity = await GetOwnedItemAsync(id);

        dto.ApplyTo(entity);

        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        return entity.ToGetDto();
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await GetOwnedItemAsync(id);

        // Soft delete.
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task<ToDoItemGetDto> ToggleCompleteAsync(long id)
    {
        var entity = await GetOwnedItemAsync(id);

        entity.IsCompleted = !entity.IsCompleted;
        entity.CompletedAt = entity.IsCompleted ? DateTime.UtcNow : null;
        entity.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        return entity.ToGetDto();
    }

    private async Task<ToDoItem> GetOwnedItemAsync(long id)
    {
        var userId = GetCurrentUserId();

        var entity = _repository.GetAllQuery()
            .FirstOrDefault(x => x.ToDoItemId == id && x.UserId == userId);

        if (entity == null)
        {
            throw new NotFoundException($"To-do item {id} was not found.");
        }

        return entity;
    }

    private long GetCurrentUserId()
    {
        var userId = _currentUserService.UserId;
        if (userId is null)
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        return userId.Value;
    }

    private static IQueryable<ToDoItem> ApplySorting(IQueryable<ToDoItem> query, ToDoItemQueryParams p)
    {
        return (p.SortBy, p.SortDescending) switch
        {
            (ToDoItemSortBy.DueDate, true) => query.OrderByDescending(x => x.DueDate),
            (ToDoItemSortBy.DueDate, false) => query.OrderBy(x => x.DueDate),
            (ToDoItemSortBy.Priority, true) => query.OrderByDescending(x => x.Priority),
            (ToDoItemSortBy.Priority, false) => query.OrderBy(x => x.Priority),
            (ToDoItemSortBy.Title, true) => query.OrderByDescending(x => x.Title),
            (ToDoItemSortBy.Title, false) => query.OrderBy(x => x.Title),
            (_, true) => query.OrderByDescending(x => x.CreatedAt),
            (_, false) => query.OrderBy(x => x.CreatedAt)
        };
    }
}
