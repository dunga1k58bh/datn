
using System.Collections.Generic;
using IdentityServer.Models;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer.Models{

    public class UserDetailsViewModel
    {
         public ApplicationUser User { get; set; }
        public List<string> RoleIds { get; set; }
        public List<ApplicationRole> Roles { get; set; }

    }
}

