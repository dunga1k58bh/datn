using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models{

    public class ApiScopeCreateViewModel
    {
        [Required(ErrorMessage = "Api key is required.")]
        [RegularExpression("^[a-zA-Z0-9_]*$", ErrorMessage = "Name should only contain letters, numbers, and underscores.")]
        public string Name {get; set;}

        [Required(ErrorMessage = "Name is required.")]
        public string DisplayName {get; set; }
        public string Description{get; set;}
    }
}

