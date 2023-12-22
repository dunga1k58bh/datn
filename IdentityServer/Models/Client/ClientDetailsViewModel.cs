
using System.Collections.Generic;
using IdentityServer.Models;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer.Models{

    public class ClientDetailsViewModel
    {

         public Client Client { get; set; }
        public List<string> RoleIds { get; set; }
        public List<string> RedirectUris { get; set; }
        public List<ApplicationRole> Roles { get; set; }

    }
}

