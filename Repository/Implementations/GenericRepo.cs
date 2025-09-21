
using CourseEnrollment.Data;
using CourseEnrollment.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CourseEnrollment.Repository.Interfaces;

public class GenericRepo<T> : IGenericRepo<T> where T : class 
{
    public readonly AppDbContext _context;
    public readonly DbSet<T> set;

    public GenericRepo(AppDbContext context)
    {
        this._context = context;
        this.set = _context.Set<T>();
    }


    public async Task<T> Create(T entity)
    {
        await set.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> Delete(Guid id)
    {
        var entity = await set.FindAsync(id);
        if (entity is null)
            return false;
        set.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<T>> GetAll()
    {
        var result = await set.ToListAsync();
        return result;
    }

    public async Task<T?> GetById(Guid id)
    {
        var entity = await set.FindAsync(id);
        if (entity is null)
            return null;
        return entity;
    }
}