using TechSavvy.Models;

namespace TechSavvy.Repository.Components
{
    public class ProductWithRatingViewModel
    {
        public ProductModel Product { get; set; }
        public int AverageStar { get; set; }
    }
}
