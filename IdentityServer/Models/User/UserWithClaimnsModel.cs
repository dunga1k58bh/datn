using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Models{

    public class UserWithClaimnsModel
    {
        public ApplicationUser user {get; set; }

        public string Name;
    }
}

