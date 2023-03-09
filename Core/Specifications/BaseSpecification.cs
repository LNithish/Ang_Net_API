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

        //setting beloww sorting fields private so that value will only be assigned inside this class
        //sorting fileds
        public Expression<Func<T, object>> OrderBy { get; private set;}

        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        //Pagination fields
        public int Take { get; private set; }

        public int Skip { get; private set; }

        public bool IsPagingEnabled { get; private set; }

        //Used when an expression is passed ,like product ID, brandID, typeID
        public BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            // criteria matches where condition
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
        //Sorting methods
        protected void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy=orderByExpression;
        }
        protected void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression;
        }
        //Pagination method
        protected void ApplyPaging(int Skip,int Take)
        {
            this.Take = Take;
            this.Skip = Skip;
            IsPagingEnabled = true;
        }
    }
}
