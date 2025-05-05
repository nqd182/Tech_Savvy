using System.ComponentModel.DataAnnotations;

namespace TechSavvy.Models
{
    public class CartItemModel // 1 sản phẩm trong giỏ
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total {
            get { return Quantity * Price; }
              }
        public CartItemModel()
        {

        }
        public CartItemModel(ProductModel product) // constructor chuyển đổi ProductModel thành CartItemModel khi thêm vào giỏ hàng.
        {
            ProductId = product.Id;
            ProductName = product.Name;
            Price = product.Price;
            Quantity = 1;
            Image = product.Image;
        }
    }
}
