using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using noHRforIT.Business.Services;
using noHRforIT.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace noHRforIT.Business.Extensions
{
    public static class ApplicationBLDependencies
    {
        public static IServiceCollection AddApplicationDbContext(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddScoped<IAuthService, AuthService>();

            return ApplicationDALDependencies.AddDbContext(serviceCollection);
        }
    }
}
