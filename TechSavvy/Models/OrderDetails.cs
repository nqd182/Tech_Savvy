using System.ComponentModel.DataAnnotations.Schema;

namespace TechSavvy.Models
{
    public class OrderDetails //Là chi tiết từng sản phẩm trong đơn hàng
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }

    }
}
