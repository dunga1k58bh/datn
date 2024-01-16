using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models{

    public class DoResetPasswordViewModel
    {
        public string UserId {get; set; }
        public ApplicationUser User {get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword {get; set; }

        public string Code {get; set;}
    }
}

