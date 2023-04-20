using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.Extensions.Configuration;
using Stripe;
//using alias for product as there is one more product present in Stripe
using Product = Core.Entities.Product;

namespace Infrastructure.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository _basketRepository;
        //unit of work can only be replace multiple Igeneric repository injection,
        //That is why Ibasketrepository can't be replaced with unit of work 
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration configuration;

        public PaymentService(IBasketRepository basketRepository, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            this.configuration = configuration;
        }

        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = configuration["StripeSettings:Secretkey"];
            var basket=await _basketRepository.GetBasketAsync(basketId);
            //returning null if basket is empty
            if (basket == null)
            {
                return null;
            }
            //m is money
            var shippingPrice = 0m;
            if (basket.DeliveryMethodId>0)
            {
                //get price for deliverymethod user selected
                var deliveryMethod=await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(basket.DeliveryMethodId);
                shippingPrice = deliveryMethod.Price;
            }
            //loop through basket items
            foreach(var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                if (item.Price != product.Price)
                {
                    item.Price=product.Price;
                }
            }

            //Hooking stripe payment intent service
            var service = new PaymentIntentService();
            PaymentIntent intent;
            //checking if we are updating payment intent or creating new one for basket
            if(string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var option = new PaymentIntentCreateOptions
                {
                    //stripe doesnt support decimal
                    Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)shippingPrice * 100,
                    Currency = "inr",
                    //Description="Test payment",
                    PaymentMethodTypes = new List<string> { "card" }
                };
                intent =await service.CreateAsync(option);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var option = new PaymentIntentUpdateOptions
                {
                    //stripe doesnt support decimal
                    Amount = (long)basket.Items.Sum(i => i.Quantity * (i.Price * 100)) + (long)shippingPrice * 100
                };
                await service.UpdateAsync(basket.PaymentIntentId, option);
            }
            await _basketRepository.UpdateBasketAsync(basket);
            return basket;
        }

        public async Task<Order> UpdateOrderPaymentFailed(string paymentIntentId)
        {
            var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
            var order=await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
            if(order==null)
            {
                return null;
            }
            order.Status=OrderStatus.PaymentFailed;
            //To update in DB
            await _unitOfWork.Complete();
            return order;
        }

        public async Task<Order> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {
            var spec = new OrderByPaymentIntentIdSpecification(paymentIntentId);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
            if (order == null)
            {
                return null;
            }
            order.Status = OrderStatus.PaymentReceived;
            //To update in DB
            await _unitOfWork.Complete();
            return order;
        }
    }
}
