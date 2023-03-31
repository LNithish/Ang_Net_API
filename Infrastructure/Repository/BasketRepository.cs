using Core.Entities;
using Core.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;
        //Injecting IConnectionMultiplexer API from Redis
        public BasketRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            return await _database.KeyDeleteAsync(basketId);
        }

        public async Task<CustomerBasket> GetBasketAsync(string basketId)
        {
            //Basket is stored as a string in Redis DB
            //Client sends a Json objects,Serialize that to String
            var data = await _database.StringGetAsync(basketId);
            //DeSerialize as CustomerBasket before sending back
            return data.IsNullOrEmpty?null:JsonSerializer.Deserialize<CustomerBasket>(data);
        }
        //for adding/updating basket
        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket customerBasket)
        {
            //if key(ID) exists,overwrites the existing basket with new basket
            //Otherwise craetes new
            var created=await _database.StringSetAsync(customerBasket.Id,
                //Keeping the Baskt for 30 days
                JsonSerializer.Serialize(customerBasket),TimeSpan.FromDays(30));
            if(!created)
            {
                return null;
            }
            return await GetBasketAsync(customerBasket.Id);
        }
    }
}
