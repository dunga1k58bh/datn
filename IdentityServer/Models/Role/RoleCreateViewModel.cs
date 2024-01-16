
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IdentityServer.Models;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer.Models{

    public class RoleCreateViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string DisplayName { get; set; }
        public string Name {get; set;}
        public string Description {get; set; }
    }
}

