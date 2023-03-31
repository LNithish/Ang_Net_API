using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class CustomerBasket
    {
        //ID will be generated from Customer/Angular Client
        public string Id { get; set; }
        public List<BasketItem> Items { get; set; }= new List<BasketItem>();
        //Empty constructer is for letting customer to create a Basket without having an ID
        public CustomerBasket()
        {
        }
        //no need of Items property as it is initialized
        public CustomerBasket(string id)
        {
            Id = id;
        }
    }
}
