using Core.Entities;

namespace Core.Interfaces
{
    //To dispose our context oce transaction is completed Implemeting Idisposable
    public interface IUnitOfWork:IDisposable
    {
        IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        //to return number of changes to our database
        Task<int> Complete();
    }
}
