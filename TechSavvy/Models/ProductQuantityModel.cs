using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechSavvy.Models
{
    public class ProductQuantityModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập số lượng sản phẩm")]
        public int Quantity { get; set; }
        public int ProductId { get; set; }
        public DateTime DateCreated { get; set; }
        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
    }
}
