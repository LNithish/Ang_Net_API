using Core.Entities;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        //Specification evaluator takes list of query/specification and evaluate them and generates Iqueryable.
        //This Iqueryable will be returned to create List of expressions/values 
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQery
            ,ISpecification<TEntity> specification)
        {
            var query = inputQery;

            //where condition
            if(specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);//p=>p.ProductTypeId==Id
            }

            //sorting

            if (specification.OrderBy != null)
            {
                query = query.OrderBy(specification.OrderBy);
            }
            if (specification.OrderByDescending != null)
            {
                query = query.OrderByDescending(specification.OrderByDescending);
            }

            //pagination 
            //paging operation should be done after criteria/where and ordr/sort condition
            if(specification.IsPagingEnabled==true)
            {
                query = query.Skip(specification.Skip).Take(specification.Take);
            }
            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

            return query;
        }
    }
}
