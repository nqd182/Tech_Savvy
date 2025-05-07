using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechSavvy.Repository.Validation;

namespace TechSavvy.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }
        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập tên sản phẩm từ 4 ký tự trở lên")]
        public string Name { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập giá sản phẩm")]
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        [Required(ErrorMessage = "Yêu cầu thêm ảnh sản phẩm")]
        public string Image { get; set; } = "test.jpg";
        public int? Quantity { get; set; }
        public int? Sold { get; set; }
        public CategoryModel Category { get; set; }
        public BrandModel Brand { get; set; }
        public RatingModel Ratings { get; set; }
        [NotMapped] // ko ánh xạ vào csdl
        [FileExtension]
        public IFormFile ImageUpload  { get; set; }
    }
}
