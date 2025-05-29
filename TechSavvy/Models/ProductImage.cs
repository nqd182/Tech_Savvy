namespace TechSavvy.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ImagePath { get; set; }
        public bool IsMain { get; set; }

        public virtual ProductModel Product { get; set; }
    }

}
