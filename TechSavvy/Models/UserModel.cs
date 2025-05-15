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

        [DataType(DataType.Password), Required(ErrorMessage = "Hãy nhập mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Hãy nhập năm sinh")]
        [Range(1900, 2100, ErrorMessage = "Năm sinh không hợp lệ")]
        public int BirthYear { get; set; }

        [Required(ErrorMessage = "Hãy chọn giới tính")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Hãy nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }
    }
}