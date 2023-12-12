using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

public class Seeder
{
    public static async Task SeedSuperAdmin(IServiceCollection services){

        var serviceProvider = services.BuildServiceProvider();
        var userManager = serviceProvider.GetService<UserManager<ApplicationUser>>();
        var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

        var username = "dungle2201";
        var email = "dunga1k58bh@gmail.com";
        var password = "Dungle2201@";

        var admin = await userManager.FindByEmailAsync(email);

        if (admin == null){
            admin = new ApplicationUser{
                UserName = username,
                Email = email
            };

            var result = userManager.CreateAsync(admin, password);

            if (result.Result.Succeeded){

                var adminRole = "superadmin";

                if (!await roleManager.RoleExistsAsync(adminRole)){

                   await roleManager.CreateAsync(new IdentityRole(adminRole));
                }  

                await userManager.AddToRoleAsync(admin, adminRole);
            } else {
                //Hanlde errors
                throw new Exception($"Error when creating super admin user");
            }
        }
    }
}