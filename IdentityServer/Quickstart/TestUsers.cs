// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using IdentityServer4;

namespace IdentityServerHost.Quickstart.UI
{
    public class TestUsers
    {
        public static List<TestUser> Users
        {
            get
            {
                var address = new
                {
                    street_address = "128C Dai La",
                    locality = "Hai Ba Trung",
                    postal_code = 10000,
                    country = "Vietnam"
                };
                
                return new List<TestUser>
                {
                    new TestUser
                    {
                        SubjectId = "818727",
                        Username = "admin",
                        Password = "Dungle2201@",
                        Claims =
                        {
                            new Claim(JwtClaimTypes.Name, "Dung Le"),
                            new Claim(JwtClaimTypes.GivenName, "Dung"),
                            new Claim(JwtClaimTypes.FamilyName, "Le"),
                            new Claim(JwtClaimTypes.Email, "dunga1k58bh@gmail.com"),
                            new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                            new Claim(JwtClaimTypes.WebSite, "http://dunga1k58bh.com"),
                            new Claim(JwtClaimTypes.Address, JsonSerializer.Serialize(address), IdentityServerConstants.ClaimValueTypes.Json),
                            new Claim("role", "admin")
                        }
                    },
                };
            }
        }
    }
}