namespace Core.Entities
{
    public class Product: BaseEntity
    {
        //Below field is covered from base class
        //public int Id { get; set; }
        //Nullale property has been disabled in Core.csproj file, no need to specify '?' or set null reference
        // public string? Summary { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PictureUrl { get; set; }
        public ProductType ProductType { get; set; }
        public int ProductTypeId { get; set; }
        public ProductBrand ProductBrand { get; set; }
        public int ProductBrandId { get; set; }
    }
}
