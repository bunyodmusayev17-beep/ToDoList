namespace ToDoList.Application.Abstractions;

public interface IBaseRepository<T> where T : class
{
    IQueryable<T> GetAllQuery();
    Task<T?> GetByIdAsync(params object[] keyValues);
    Task AddAsync(T t);
    void Update(T t);
    void Delete(T t);
    Task<int> SaveChangesAsync();
}
