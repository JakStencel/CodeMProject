using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using noHRforIT.Data;
using noHRforIT.Data.Models;
using noHRforIT.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace noHRforIT.Data.Data
{
    public static class SeedDb
    {
        public static async Task CreateRoles(IServiceProvider serviceProvider)
        {
            Role[] roles = new Role[] { Role.Admin, Role.Developer, Role.Manager};

            using (var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>())
            {
                foreach (var roleName in roles)
                {
                    if (!(await roleManager.RoleExistsAsync(roleName.ToString())))
                    {
                        await roleManager.CreateAsync(new IdentityRole(roleName.ToString()));
                    }
                }
            }
            
            using (var userManager = serviceProvider.GetRequiredService<UserManager<UserDTO>>())
            {
                var user = new UserDTO
                {
                    UserName = Environment.GetEnvironmentVariable("SuperUserName")
                };

                string userPassword = Environment.GetEnvironmentVariable("SuperUserPassword");
                
                if ((await userManager.FindByNameAsync(Environment.GetEnvironmentVariable("SuperUserName"))) == null)
                {
                    if ((await userManager.CreateAsync(user, userPassword)).Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                }
            }
        }
    }
}
