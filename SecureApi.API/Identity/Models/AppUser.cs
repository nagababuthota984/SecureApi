using Microsoft.AspNetCore.Identity;

namespace SecureApi.API.Identity.Models
{
    public class AppUser : IdentityUser
    {
        public override string Id { get; set; }
    }

    public enum AppUserRoles
    {
        Admin,
        SuperAdmin,
        User
    }
}
