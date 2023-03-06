using API.DTOs;
using AutoMapper;
using Core.Entities;

namespace API.Helpers
{
    public class ProductUrlResolver : IValueResolver<Product, ProductToReturnDto, string>
    {
        //configuration is to read APIurl we defind in appsetting.json
        public readonly IConfiguration Configuration;

        public ProductUrlResolver(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        //adding apiurl with the pictureUrl property to access the image
        public string Resolve(Product source, ProductToReturnDto destination,
            string destMember, ResolutionContext context)
        {
            if(!String.IsNullOrEmpty(source.PictureUrl))
            {
                return Configuration["ApiUrl"]+source.PictureUrl;
            }
            return null;
        }
    }
}
