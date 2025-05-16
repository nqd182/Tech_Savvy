using TechSavvy.Models;

namespace TechSavvy.Models.ViewModels
{
    public class UserWithRoleViewModel
    {
        public AppUserModel User { get; set; }
        public string RoleName { get; set; }
    }
}
