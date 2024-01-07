
using System.Collections.Generic;
using IdentityServer.Models;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer.Models{

    public class RoleDetailsViewModel
    {
         public ApplicationRole Role { get; set; }
        public List<string> UserIds { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public string Description {get; set; }

    }
}

