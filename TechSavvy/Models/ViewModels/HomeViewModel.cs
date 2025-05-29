namespace TechSavvy.Models.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<ProductModel> NewProducts { get; set; }
        public IEnumerable<ProductModel> TopSellingProducts { get; set; }
        public IEnumerable<ProductModel> WidgetTopSellingProducts { get; set; }
    }
}
