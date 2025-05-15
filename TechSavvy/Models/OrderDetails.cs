using System.ComponentModel.DataAnnotations.Schema;

namespace TechSavvy.Models
{
    public class OrderDetails //Là chi tiết từng sản phẩm trong đơn hàng
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; } // giá sản phẩm tại thời điểm đặt hàng
        public int Quantity { get; set; }
        public decimal Total => UnitPrice * Quantity; // ko lưu vào csdl
        public bool IsDeleted { get; set; } = false;

        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
    }
}
