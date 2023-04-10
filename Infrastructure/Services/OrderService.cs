using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        //private readonly IGenericRepository<Order> orderRepo;
        //private readonly IGenericRepository<Product> productRepo;
        //private readonly IBasketRepository basketRepository;
        //private readonly IGenericRepository<DeliveryMethod> dmRepo;

        //public OrderService(IGenericRepository<Order> orderRepo,IGenericRepository<Product> productRepo
        //    ,IBasketRepository basketRepository,IGenericRepository<DeliveryMethod> dmRepo)
        //{
        //    this.orderRepo = orderRepo;
        //    this.productRepo = productRepo;
        //    this.basketRepository = basketRepository;
        //    this.dmRepo = dmRepo;
        //}

        //Repalcing above injection with UnitOfWork
        private readonly IUnitOfWork unitOfWork;
        private readonly IBasketRepository basketRepository;
        public OrderService(IUnitOfWork unitOfWork, IBasketRepository basketRepository)
        {
            this.unitOfWork = unitOfWork;
            this.basketRepository = basketRepository;
        }


        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId
            , Address shippingAddress)
        {
            //get our basket from basket repository
            var basket = await basketRepository.GetBasketAsync(basketId);
            //get productitem from product repository and create OrderItems for the order
            var items = new List<OrderItem>();
            foreach(var item in basket.Items)
            {
                //var productItem = await productRepo.GetByIdAsync(item.Id);
                var productItem = await unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);
                items.Add(orderItem);
            }
            //get the delivery method
            //var deliveryMethods = await dmRepo.GetByIdAsync(deliveryMethodId);
            var deliveryMethods = await unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
            //calculate subtotal
            var subTotal = items.Sum(item => item.Price * item.Quantity);
            //create order and save to DB and return the order
            var order=new Order(items,buyerEmail,shippingAddress,deliveryMethods,subTotal);
            unitOfWork.Repository<Order>().Add(order);
            var results = await unitOfWork.Complete();
            if(results<=0)
            {
                return null;
            }
            //delete basket
            await basketRepository.DeleteBasketAsync(basketId);
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(id,buyerEmail);
            return await unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrdersWithItemsAndOrderingSpecification(buyerEmail);
            return await unitOfWork.Repository<Order>().ListAsync(spec);
        }
    }
}
