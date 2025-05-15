using Microsoft.AspNetCore.Identity;

namespace TechSavvy.Models
{
    public class AppUserModel : IdentityUser
    {
        public string Occupation { get; set; }
        public string RoleId { get; set; }
        public bool IsDeleted { get; set; }

        public int BirthYear { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
    }
}
