using System.ComponentModel.DataAnnotations;

namespace TechSavvy.Models
{
    public class CouponModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên mã giảm giá")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập mô tả")]
        public string Description { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateExpired { get; set; }
        [Required(ErrorMessage = "Yêu cầu giá coupon")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Yêu cầu số lượng coupon")]
        public int Quantity { get; set; }
        public int Status { get; set; }
        public bool IsDeleted { get; set; } = false;

    }
}
