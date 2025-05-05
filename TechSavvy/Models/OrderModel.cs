namespace TechSavvy.Models
{
    public class OrderModel //Đại diện cho 1 giao dịch mua hàng của khách
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }

        public int Status { get; set; }

    }
}
