// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using IdentityServer4.EntityFramework.Storage;
using Serilog;
using System.Security.Claims;
using IdentityServer.Data;
using IdentityServer.Models;
using System;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.EntityFramework.Entities;

namespace IdentityServer
{
    public class SeedData
    {
        public static void EnsureSeedData(string connectionString)
        {
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddDbContext<ApplicationDbContext>(options =>
               options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddOperationalDbContext(options =>
            {
                options.ConfigureDbContext = db => db.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
            });
            services.AddConfigurationDbContext(options =>
            {
                options.ConfigureDbContext = db => db.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
            });

            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {

                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                context.Database.Migrate();
                EnsureSeedData(context);

                EnsureSeedUserData(context, scope);
            }


        }

        private static void EnsureSeedData(ApplicationDbContext context)
        {

            Log.Debug("Clients being populated");
            foreach (var client in Config.Clients.ToList())
            {
                Log.Information("Logging an object: {@MyObject}", client);
                var a = context.Clients.FirstOrDefault(i => i.ClientId == client.ClientId);
                if(a != null){
                    continue;
                }
                context.Clients.Add(client.ToEntity());
            }
            context.SaveChanges();

            Log.Debug("IdentityResources being populated");
            foreach (var resource in Config.IdentityResources.ToList())
            {
                var a = context.IdentityResources.FirstOrDefault(i => i.Name == resource.Name);
                if(a != null){
                    continue;
                }

                context.IdentityResources.Add(resource.ToEntity());
            }
            context.SaveChanges();
        

            Log.Debug("ApiScopes being populated");
            foreach (var resource in Config.ApiScopes.ToList())
            {

                var existApiScope = context.ApiScopes.FirstOrDefault(i => i.Name == resource.Name);
                if(existApiScope != null){
                    continue;
                }

                context.ApiScopes.Add(resource.ToEntity());
            }
            context.SaveChanges();
        }


        private static void EnsureSeedUserData(ApplicationDbContext context, IServiceScope scope)
        {

            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            //Hard code
            var alice = userMgr.FindByNameAsync("alice").Result;
            if (alice == null)
            {
                alice = new ApplicationUser
                {
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                };
                var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(alice, new Claim[]{
                    new Claim(JwtClaimTypes.Name, "Alice Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Alice"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("alice created");
            }
            else
            {
                Log.Debug("alice already exists");
            }

            var bob = userMgr.FindByNameAsync("bob").Result;
            if (bob == null)
            {
                bob = new ApplicationUser
                {
                    UserName = "bob",
                    Email = "BobSmith@email.com",
                    EmailConfirmed = true
                };
                var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(bob, new Claim[]{
                    new Claim(JwtClaimTypes.Name, "Bob Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Bob"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                    new Claim("location", "somewhere")
                }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("bob created");
            }
            else
            {
                Log.Debug("bob already exists");
            }
        }
    }
}
