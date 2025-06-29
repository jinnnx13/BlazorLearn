namespace Blazor1.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double price { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<ProductPropModel> ProductProperties { get; set; }
    }
}
