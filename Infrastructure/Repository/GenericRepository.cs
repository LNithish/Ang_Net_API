﻿using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    //Generic repository will have basic implementation for all entity
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly StoreContext storeContext;

        public GenericRepository(StoreContext storeContext)
        {
            this.storeContext = storeContext;
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var data=await storeContext.Set<T>().FindAsync(id);
            return data;
        }

        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            var data= await storeContext.Set<T>().ToListAsync();
            return data;
        }

        //Methods based on specification
        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).ToListAsync();
        }
        
        public async Task<T> GetEntityWithSpec(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync();
        }

        public async Task<int> CountAsync(ISpecification<T> specification)
        {
            return await ApplySpecification(specification).CountAsync();
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(storeContext.Set<T>().AsQueryable(), spec);
        }

        public void Add(T entity)
        {
            //It will begin tracking the entity and its any reachable entity & update DB once savechanges called
            storeContext.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            storeContext.Set<T>().Attach(entity);
            storeContext.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            storeContext.Set<T>().Remove(entity);
        }
    }
}
