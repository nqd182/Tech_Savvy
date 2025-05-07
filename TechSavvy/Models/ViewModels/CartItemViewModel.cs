namespace TechSavvy.Models.ViewModels
{
    public class CartItemViewModel // toàn bộ sản phẩm trong giỏ
    {
        public List<CartItemModel> CartItems { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal ShippingCost { get; set; }
        public string CouponCode { get; set; }

    }
}
