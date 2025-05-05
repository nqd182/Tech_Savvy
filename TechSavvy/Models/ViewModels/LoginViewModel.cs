using System.ComponentModel.DataAnnotations;

namespace TechSavvy.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Hãy nhập tên người dùng")]
        public string UserName { get; set; }
       
        [DataType(DataType.Password), Required(ErrorMessage = "Hãy nhập mật khẩu")] // mã hóa password
        public string Password { get; set; }
        public string ReturnUrl { get; set; } // Giữ lại đường dẫn gốc để chuyển hướng sau khi đăng nhập
    }
}
