namespace Core.Entities
{
    public class Product
    {
        public int Id { get; set; }
        //Nullale property has been disabled in Core.csproj file, no need to specify '?' or set null reference
        // public string? Summary { get; set; }
        public string Name { get; set; }
    }
}
