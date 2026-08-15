using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Application.Dtos;
using ToDoList.Application.Services;

namespace ToDoList.Api.Controllers;

[Route("api/v1/todoitems")]
[ApiController]
[Authorize]
public class ToDoItemsController : ControllerBase
{
    private readonly IToDoItemService _toDoItemService;

    public ToDoItemsController(IToDoItemService toDoItemService)
    {
        _toDoItemService = toDoItemService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ToDoItemGetDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ToDoItemGetDto>>> GetAll([FromQuery] ToDoItemQueryParams query)
    {
        var result = await _toDoItemService.GetAllAsync(query);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ToDoItemGetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ToDoItemGetDto>> GetById(long id)
    {
        var result = await _toDoItemService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ToDoItemGetDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ToDoItemGetDto>> Create([FromBody] ToDoItemCreateDto dto)
    {
        var created = await _toDoItemService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.ToDoItemId }, created);
    }

    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ToDoItemGetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ToDoItemGetDto>> Update(long id, [FromBody] ToDoItemUpdateDto dto)
    {
        var updated = await _toDoItemService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id)
    {
        await _toDoItemService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPatch("{id:long}/complete")]
    [ProducesResponseType(typeof(ToDoItemGetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ToDoItemGetDto>> ToggleComplete(long id)
    {
        var result = await _toDoItemService.ToggleCompleteAsync(id);
        return Ok(result);
    }
}
