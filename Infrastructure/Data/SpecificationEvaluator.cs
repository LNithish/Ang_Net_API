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
        //This Iqueryable will be resturned to create List of expressions 
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQery
            ,ISpecification<TEntity> specification)
        {
            var query = inputQery;
            if(specification.Criteria != null)
            {
                query = query.Where(specification.Criteria);//p=>p.ProductTypeId==Id
            }

            query = specification.Includes.Aggregate(query, (current, include) => current.Include(include));

            return query;
        }
    }
}
