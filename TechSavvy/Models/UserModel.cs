using System.ComponentModel.DataAnnotations;

namespace TechSavvy.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Hãy nhập tên người dùng")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Hãy nhập email"), EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password), Required(ErrorMessage = "Hãy nhập mật khẩu")] // mã hóa password
        public string Password { get; set; }

    }
}
