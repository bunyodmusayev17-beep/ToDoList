using ToDoList.Application.Dtos;

namespace ToDoList.Application.Abstractions;

public interface ITokenService
{
    string GetToken(UserGetDto userGetDto);
    string GenerateRefreshToken();
}
