using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechSavvy.Repository.Validation;

namespace TechSavvy.Models
{
    public class ContactModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Yêu cầu tiêu đề website ")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập bản đồ website ")]
        public string Map { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập email liên hệ")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Yêu cầu mô tả website ")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập số điện thoại liên hệ ")]
        public string Phone { get; set; }
        public string LogoImg { get; set; }

        [NotMapped] // ko ánh xạ vào csdl
        [FileExtension]
        public IFormFile ImageUpload { get; set; }


    }
}
