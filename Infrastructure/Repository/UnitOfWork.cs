using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using System.Collections;

namespace Infrastructure.Repository
{
    //UnitOfwork will have collection of all the repositories and its DBContext
    public class UnitOfWork : IUnitOfWork
    {
        private readonly StoreContext context;
        private Hashtable _repositories;

        //Unit of work will be responsible for creating instance of storeContext
        //it will use single storecontext to use for multiple repositories
        public UnitOfWork(StoreContext context)
        {
            this.context = context;
        }

        public async Task<int> Complete()
        {
            return await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            if(_repositories==null)
            {
                _repositories = new Hashtable();
            }
            //get the type of entity
            var type= typeof(TEntity).Name;
            //check the hash table for name of the entity type
            if(!_repositories.ContainsKey(type))
            {
                //specify repository type
                var repositoryType= typeof(GenericRepository<>);
                //if we don't have the repository for this particular type, We will create instance of this
                var repositoryInstance=Activator.CreateInstance(repositoryType.MakeGenericType(
                    typeof(TEntity)),context);

                //add the new entry to hashtable for the new repository type
                _repositories.Add(type, repositoryInstance);
            }
            return (IGenericRepository<TEntity>)_repositories[type];
        }
    }
}
