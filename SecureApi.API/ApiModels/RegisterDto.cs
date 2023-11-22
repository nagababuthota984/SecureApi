using System.ComponentModel.DataAnnotations;

namespace SecureApi.API.ApiModels
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Username is required")]
        [StringLength(15)]
        public string Username { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}
