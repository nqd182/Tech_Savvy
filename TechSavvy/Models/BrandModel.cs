using System.ComponentModel.DataAnnotations;

namespace TechSavvy.Models
{
    public class BrandModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập tên thương hiệu")]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }
        public int Status { get; set; }
    }
}
