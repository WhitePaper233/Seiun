using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Minio;
using Seiun.Entities;

namespace Seiun.Repositories;

public class BaseRepository<T>(SeiunDbContext dbContext, IMinioClient minioClient)
	: IBaseRepository<T> where T : BaseEntity
{
	public SeiunDbContext DbContext { get; set; } = dbContext;
	public IMinioClient MinioCl { get; set; } = minioClient;

	public void Create(T entity)
	{
		DbContext.Set<T>().Add(entity);
	}

	public void Delete(T entity)
	{
		DbContext.Set<T>().Remove(entity);
	}

	public void Update(T entity)
	{
		DbContext.Set<T>().Update(entity);
	}

	public async Task<IEnumerable<T>> GetAllAsync() => await DbContext.Set<T>().ToListAsync();

	public async Task<IEnumerable<T>> GetByConditionAsync(Expression<Func<T, bool>> expression) => await DbContext.Set<T>().Where(expression).ToListAsync();

	public async Task<IEnumerable<T>> GetByGuidsAsync(IEnumerable<Guid> guids)
	{
		return await DbContext.Set<T>().Where(record => guids.Contains(record.Id)).ToListAsync();
	}

	public async Task<T?> GetByIdAsync(Guid guid) => await DbContext.Set<T>().FindAsync(guid);

	public async Task<bool> SaveAsync() => await DbContext.SaveChangesAsync() > 0;
}
