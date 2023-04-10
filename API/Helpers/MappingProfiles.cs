//Using Automapper.extensions.microsoft.dependencyinjection to replace manual DTO object codes
using API.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;
using Core.Entities.OrderAggregate;

namespace API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                //To let automapper know some properties in Product class are having entity as its type
                .ForMember(d=>d.ProductBrand,option=>option.MapFrom(source=>source.ProductBrand.Name))
                .ForMember(d=>d.ProductType, option=>option.MapFrom(source=>source.ProductType.Name))

            //adding pictureUrl with api url
           .ForMember(d => d.PictureUrl, option => option.MapFrom<ProductUrlResolver>());

            //Creating MAP for AddressDto, ReverseMap allows vice versa also
            CreateMap<Core.Entities.Identity.Address, AddressDto>().ReverseMap();

            //Map for Basket
            //Automapper will take care of mapping basketitem in CustomerBasketDto from BasketItemDto
            CreateMap<BasketItemDto, BasketItem>();
            CreateMap<CustomerBasketDto, CustomerBasket>();

            //mapping for order creation
            CreateMap<AddressDto, Core.Entities.OrderAggregate.Address>();

            //map for returning order details
            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DeliveryMethod, option => option.MapFrom(s => s.DeliveryMethod.ShortName))
                .ForMember(d => d.ShippingPrice, option => option.MapFrom(s => s.DeliveryMethod.Price));
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ItemOrdered.ProductItemId))
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ItemOrdered.ProductName))
                .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.ItemOrdered.PictureUrl))
                .ForMember(d=>d.PictureUrl,o=>o.MapFrom<OrderItemPictureUrlResolver>());
        }
    }
}
