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

            services.AddIdentity<ApplicationUser, ApplicationRole>()
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
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            var admin_role = roleMgr.FindByNameAsync("admin").Result;
            if (admin_role == null){
                admin_role = new ApplicationRole{
                    Name = "admin"
                };

                var result = roleMgr.CreateAsync(admin_role).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }

            var basic_role = roleMgr.FindByNameAsync("basic").Result;
            if (basic_role == null){
                basic_role = new ApplicationRole{
                    Name = "basic"
                };

                var result = roleMgr.CreateAsync(basic_role).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
            }

            //Hard code
            var admin = userMgr.FindByNameAsync("admin").Result;
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "dunga1k58bh@gmail.com",
                    EmailConfirmed = true
                };
                var result = userMgr.CreateAsync(admin, "Pass123$").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userMgr.AddClaimsAsync(admin, new Claim[]{
                    new Claim(JwtClaimTypes.Name, "Admin Super"),
                    new Claim(JwtClaimTypes.GivenName, "Admin"),
                    new Claim(JwtClaimTypes.FamilyName, "Super"),
                    new Claim(JwtClaimTypes.WebSite, "https://identityserver22.azurewebsites.net/"),
                    new Claim("location", "Ha noi")
                }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                Log.Debug("admin created");
            }
            else
            {
                Log.Debug("admin already exists");
            }
        }
    }
}
