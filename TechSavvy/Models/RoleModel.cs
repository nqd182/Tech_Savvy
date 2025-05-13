using Microsoft.AspNetCore.Identity;

namespace TechSavvy.Models
{
    public class RoleModel : IdentityRole
    {
        public bool IsDeleted { get; set; } = false; // Xóa mềm
    }
}
