using API.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper mapper;

        //injecting basket service
        public BasketController(IBasketRepository basketRepository,IMapper mapper)
        {
            _basketRepository = basketRepository;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<CustomerBasket>> GetBasketById(string id)
        {
            var basket =await _basketRepository.GetBasketAsync(id);
            //this basket can be empty(basket without an items),
            //so if it is empty, returning a new CustomerBasket object which will have an empty list of item 
            //?? It's the null-coalescing operator.
            //It returns the first argument unless it is null, in which case it returns the second.
            return Ok(basket ?? new CustomerBasket(id));
        }
        [HttpPost]
        //Applying validation for customer passing data using DTO classs and automapper
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
        {
            var customerBasket = mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var updatedBasket = await _basketRepository.UpdateBasketAsync(customerBasket);
            return Ok(updatedBasket);
        }
        [HttpDelete]
        public async Task DeleteBasketAsync(string id)
        {
            await _basketRepository.DeleteBasketAsync(id);
        }
    }
}
