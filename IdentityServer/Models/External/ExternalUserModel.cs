
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using IdentityServer.Models;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer.Models{

    class ExternalUserModel{

        public ApplicationUser user {get; set; }
        public string provider {get; set; }
        public string providerUserId {get; set; }
        public IEnumerable<Claim> claims {get; set;}
    }
}