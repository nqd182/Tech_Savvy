using System.ComponentModel.DataAnnotations;

namespace TechSavvy.Models
{
    public class StatisticalModel
    {
        [Key]
        public int Id { get; set; }
        public int Quantity { get; set; }//số lượng bán     
        public int Sold { get; set; }//số lượng đơn hàng
        public int Status { get; set; }
        public decimal Revenue { get; set; }//doanh thu
        public decimal Profit { get; set; }
        public DateTime DateCreated { get; set; }//ngày đặt hàng


    }
}
