using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AuthMicroservice.API.Models
{
    public class AuthenticationModel
    {
        [Key]
        [Required]
        public int UserId { get; set; }
        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        [DisplayName("Email Address")]
        public string EmailAddress { get; set; }
        [Required]
        [DisplayName("Phone Number")]
        public double PhoneNumber { get; set; }
    }
}