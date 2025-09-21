namespace CourseEnrollment.Repository.Interfaces
{
    public interface IGenericRepo<T> where T :class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(Guid id);
        Task<bool> Delete(Guid id);
        Task<T> Create(T entity);
    }
}