using ToDoList.Application.Abstractions;

namespace ToDoList.Infrastructure.Persistence.Implementations;

public class BaseRepository<T> : IBaseRepository<T> where T : class
{
    private readonly AppDbContext DbContext;

    public BaseRepository(AppDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public async Task AddAsync(T t)
    {
        await DbContext.AddAsync(t);
    }

    public void Delete(T t)
    {
        DbContext.Remove(t);
    }

    public IQueryable<T> GetAllQuery()
    {
        return DbContext.Set<T>().AsQueryable();
    }

    public async Task<T?> GetByIdAsync(params object[] keyValues)
    {
        return await DbContext.Set<T>().FindAsync(keyValues);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await DbContext.SaveChangesAsync();
    }

    public void Update(T t)
    {
        DbContext.Update(t);
    }
}
