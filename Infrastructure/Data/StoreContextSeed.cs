using Core.Entities;
using Core.Entities.OrderAggregate;
using System.Text.Json;

namespace Infrastructure.Data
{
    //seeding data during start of the application
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext storeContext)
        {
            if (!storeContext.ProductBrands.Any())
            {
                var brandsData = File.ReadAllText("../Infrastructure/Data/SeedData/brands.json");
                var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);
                storeContext.ProductBrands.AddRange(brands);
            }
            if (!storeContext.ProductTypes.Any())
            {
                var TypesData = File.ReadAllText("../Infrastructure/Data/SeedData/types.json");
                var types = JsonSerializer.Deserialize<List<ProductType>>(TypesData);
                storeContext.ProductTypes.AddRange(types);
            }
            if (!storeContext.products.Any())
            {
                var ProductsData = File.ReadAllText("../Infrastructure/Data/SeedData/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(ProductsData);
                storeContext.products.AddRange(products);
            }
            //Seed data for delivery methods
            if (!storeContext.DeliveryMethods.Any())
            {
                var DeliveryData = File.ReadAllText("../Infrastructure/Data/SeedData/delivery.json");
                var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryData);
                storeContext.DeliveryMethods.AddRange(methods);
            }
            if (storeContext.ChangeTracker.HasChanges())
            {
                await storeContext.SaveChangesAsync();
            }
        }

    }
}