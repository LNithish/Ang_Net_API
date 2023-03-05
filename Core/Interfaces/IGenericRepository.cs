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
    }
}
