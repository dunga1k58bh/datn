using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models{

    public class ResetPasswordViewModel
    {
       [Required(ErrorMessage = "Email can't not be empty")]
       public string Email {get; set; }
    }
}

