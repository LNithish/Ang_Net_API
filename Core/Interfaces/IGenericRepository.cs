using Core.Entities;
using Core.Specifications;

namespace Core.Interfaces
{
    //Where condition checks whether entity(class) used in palce of T is either derived from BaseEntity or A BaseEntity
    public interface IGenericRepository<T> where T:BaseEntity
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> ListAllAsync();
        //Spcification Methods
        Task<T> GetEntityWithSpec(ISpecification<T> specification);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification);
        //To get the count of records before pagination is applied
        Task<int> CountAsync(ISpecification<T> specification);
        //below methods are not async reason is none of this are directly adding the changes to database.
        //EF will only track it, it will be in memory
        void Add(T entity);
        void Update (T entity);
        void Delete(T entity);
    }
}
