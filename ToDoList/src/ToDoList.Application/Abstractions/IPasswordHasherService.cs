namespace ToDoList.Application.Abstractions;

public interface IPasswordHasherService
{
    public (string Hash, string Salt) Hasher(string password);
    public bool Verify(string password, string hash, string salt);
}
