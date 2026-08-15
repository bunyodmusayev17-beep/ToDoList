using ToDoList.Application.Dtos;

namespace ToDoList.Application.Services;

public interface IToDoItemService
{
    Task<ToDoItemGetDto> CreateAsync(ToDoItemCreateDto dto);
    Task<ToDoItemGetDto> GetByIdAsync(long id);
    Task<PagedResult<ToDoItemGetDto>> GetAllAsync(ToDoItemQueryParams query);
    Task<ToDoItemGetDto> UpdateAsync(long id, ToDoItemUpdateDto dto);
    Task DeleteAsync(long id);
    Task<ToDoItemGetDto> ToggleCompleteAsync(long id);
}
