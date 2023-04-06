//Using Automapper.extensions.microsoft.dependencyinjection to replace manual DTO object codes
using API.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;

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
            CreateMap<Address, AddressDto>().ReverseMap();

            //Map for Basket
            //Automapper will take care of mapping basketitem in CustomerBasketDto from BasketItemDto
            CreateMap<BasketItemDto, BasketItem>();
            CreateMap<CustomerBasketDto, CustomerBasket>();

        }
    }
}
