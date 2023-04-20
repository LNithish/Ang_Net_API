using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities.OrderAggregate
{
    public class Order:BaseEntity
    {
        //parameterless constructor for EF
        public Order()
        {
        }

        public Order(IReadOnlyList<OrderItem> orderItems, string buyerEmail,Address shipToAddress, DeliveryMethod deliveryMethod
            , decimal subTotal,string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            //OrderDate is st to DateTime.UtcNow
            //OrderDate = orderDate;
            ShipToAddress = shipToAddress;
            DeliveryMethod = deliveryMethod;
            OrderItems = orderItems;
            SubTotal = subTotal;
            //Status will be set to Pending automatically for now
            //Status = status;
            //Need stripe to provide paymentintentid
            PaymentIntentId = paymentIntentId;
        }

        public string BuyerEmail { get; set; }
        public DateTime OrderDate { get; set; }= DateTime.UtcNow;
        public Address ShipToAddress { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public IReadOnlyList<OrderItem> OrderItems { get; set;}
        public decimal SubTotal { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string PaymentIntentId { get; set; }
        //When automapper sees a property with Get in its name,
        //it will execute the function we add inside and map it to property called Total
        public decimal GetTotal()
        {
            return SubTotal + DeliveryMethod.Price;
        }
    }
}
