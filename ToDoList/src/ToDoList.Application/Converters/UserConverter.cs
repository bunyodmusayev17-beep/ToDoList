using ToDoList.Application.Dtos;
using ToDoList.Domain.Entities;

namespace ToDoList.Application.Converters;

public static class UserConverter
{
    public static User ToEntity(this RegisterDto dto)
    {
        var user = new User()
        {
            UserName = dto.UserName,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Password = dto.Password,
        };

        return user;
    }

}
