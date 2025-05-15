namespace TechSavvy.Models
{
    public class OrderModel //Đại diện cho 1 giao dịch mua hàng của khách
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string UserName { get; set; }
        public string CouponCode { get; set; }
        public decimal CouponPrice { get; set; }
        public decimal ShippingCost { get; set; }
        public string? ShippingAddress { get; set; }
        public decimal TotalPrice { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Status { get; set; }
        public bool IsDeleted { get; set; } = false;


    }
}
