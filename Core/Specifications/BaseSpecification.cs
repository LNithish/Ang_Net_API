using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    //Specification pattern to address anti-pattern in IgenericRepository
    public class BaseSpecification<T> : ISpecification<T>
    {

        public Expression<Func<T, bool>> Criteria { get;}

        public List<Expression<Func<T, object>>> Includes { get;}
            =new List<Expression<Func<T, object>>>();

        //Used when product ID is passed 
        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }
        //Used when no productID is passes/no criteria for filtering
        public BaseSpecification()
        {
        }

        //custom include function to implement include in Igenericrepository
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
    }
}
