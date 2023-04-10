using Core.Entities.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.Specifications
{
    public class OrdersWithItemsAndOrderingSpecification : BaseSpecification<Order>
    {
        //to get all the orders for the user's email
        public OrdersWithItemsAndOrderingSpecification(string email):base(o=>o.BuyerEmail==email)
        {
            //Adding the entity details which need to be included while getting order details
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.DeliveryMethod);
            //sorting
            AddOrderByDescending(o => o.OrderDate);
        }

        //for individual order
        public OrdersWithItemsAndOrderingSpecification(int orderId,string email) : base(
            o=>o.Id==orderId&&o.BuyerEmail==email)
        {
            //Adding the entity details which need to be included while getting order details
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.DeliveryMethod);
        }
    }
}
