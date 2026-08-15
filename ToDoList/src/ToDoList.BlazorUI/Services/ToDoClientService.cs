using ToDoList.BlazorUI.Models;

namespace ToDoList.BlazorUI.Services;

public class ToDoClientService
{
    private const string BaseRoute = "api/v1/todoitems";

    private readonly ApiClient _api;

    public ToDoClientService(ApiClient api)
    {
        _api = api;
    }

    public Task<ApiResult<PagedResult<ToDoItemDto>>> GetAllAsync(ToDoItemQuery query)
        => _api.GetAsync<PagedResult<ToDoItemDto>>($"{BaseRoute}{query.ToQueryString()}");

    public Task<ApiResult<ToDoItemDto>> GetByIdAsync(long id)
        => _api.GetAsync<ToDoItemDto>($"{BaseRoute}/{id}");

    public Task<ApiResult<ToDoItemDto>> CreateAsync(ToDoItemCreateRequest request)
        => _api.PostAsync<ToDoItemDto>(BaseRoute, request);

    public Task<ApiResult<ToDoItemDto>> UpdateAsync(long id, ToDoItemUpdateRequest request)
        => _api.PutAsync<ToDoItemDto>($"{BaseRoute}/{id}", request);

    public Task<ApiResult> DeleteAsync(long id)
        => _api.DeleteAsync($"{BaseRoute}/{id}");

    public Task<ApiResult<ToDoItemDto>> ToggleCompleteAsync(long id)
        => _api.PatchAsync<ToDoItemDto>($"{BaseRoute}/{id}/complete");
}
