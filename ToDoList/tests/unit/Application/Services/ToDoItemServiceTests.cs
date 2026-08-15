using Moq;
using ToDoList.Application.Abstractions;
using ToDoList.Application.Dtos;
using ToDoList.Application.Services;
using ToDoList.Domain.Entities;

namespace Application.Services;

public class ToDoItemServiceTests
{
    private readonly Mock<IBaseRepository<ToDoItem>> _repository;
    private readonly Mock<ICurrentUserService> _currentUser;
    private readonly ToDoItemService _service;

    public ToDoItemServiceTests()
    {
        _repository = new Mock<IBaseRepository<ToDoItem>>();
        _currentUser = new Mock<ICurrentUserService>();

        _service = new ToDoItemService(
            _repository.Object,
            _currentUser.Object);
    }

    [Fact]
    public async Task DeleteAsync_TodoItemExists_DeletesTodoItem()
    {
        // Arrange

        var todoItem1 = new ToDoItem { ToDoItemId = 1L, UserId = 1L, Title = "Test1", IsCompleted = false };
        var todoItem2 = new ToDoItem { ToDoItemId = 2L, UserId = 2L, Title = "Test2", IsCompleted = true };
        var todoItemId = 1L;

        _currentUser.Setup(c => c.UserId).Returns(1L);

        _repository.Setup(r => r.GetAllQuery()).
            Returns(new List<ToDoItem>
            {
                todoItem1, todoItem2
            }.AsQueryable());

        _repository.Setup(r => r.Update(todoItem1));
        _repository.Setup(r => r.SaveChangesAsync());


        // Act
        await _service.DeleteAsync(todoItemId);

        // Assert
        _repository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _repository.Verify(r => r.Update(todoItem1), Times.Once);
    }


    [Fact]
    public async Task GetAllAsync_ReturnsOnlyCurrentUserTodoItems()
    {
        // Arrange
        var todoItem1 = new ToDoItem
        {
            ToDoItemId = 1,
            UserId = 1,
            Title = "Task 1",
            IsCompleted = false
        };

        var todoItem2 = new ToDoItem
        {
            ToDoItemId = 2,
            UserId = 2,
            Title = "Task 2",
            IsCompleted = true
        };

        var query = new ToDoItemQueryParams
        {
            Page = 1,
            PageSize = 10
        };

        _currentUser.Setup(x => x.UserId).Returns(1);

        _repository.Setup(x => x.GetAllQuery())
            .Returns(new List<ToDoItem>
            {
            todoItem1,
            todoItem2
            }.AsQueryable());

        // Act
        var result = await _service.GetAllAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);

        Assert.Equal(1, result.TotalCount);

        Assert.Equal(todoItem1.Title, result.Items.First().Title);

        _repository.Verify(x => x.GetAllQuery(), Times.Once);
    }


}
