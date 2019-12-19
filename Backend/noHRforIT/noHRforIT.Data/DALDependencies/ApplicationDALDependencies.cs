using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using noHRforIT.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace noHRforIT.Data
{
    public static class ApplicationDALDependencies
    {
        public static IServiceCollection AddDbContext(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContext<ApplicationDbContext>();
            serviceCollection.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

            serviceCollection.AddIdentity<UserDTO, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
                //.AddDefaultTokenProviders();

            return serviceCollection;
        }
    }
}
