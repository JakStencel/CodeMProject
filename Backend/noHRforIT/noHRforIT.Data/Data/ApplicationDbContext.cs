using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using noHRforIT.Data.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace noHRforIT.Data
{
    public interface IApplicationDbContext
    {
        DbSet<UserDTO> UsersDTO { get; set; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public class ApplicationDbContext : IdentityDbContext<UserDTO>, IApplicationDbContext
    {
        public ApplicationDbContext() : base(GetDbContextOptionBuilder().Options) { }

        public DbSet<UserDTO> UsersDTO { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserDTO>()
                .Ignore(u => u.AccessFailedCount)
                .Ignore(u => u.ConcurrencyStamp)
                .Ignore(u => u.LockoutEnabled)
                .Ignore(u => u.LockoutEnd)
                .Ignore(u => u.PhoneNumberConfirmed)
                .Ignore(u => u.TwoFactorEnabled)
                .Ignore(u => u.SecurityStamp);
        }

        private static DbContextOptionsBuilder<ApplicationDbContext> GetDbContextOptionBuilder()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(@Directory.GetCurrentDirectory() + "../../noHRforIT/appsettings.json").Build();
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("noHRConnectionString");
            builder.UseSqlServer(connectionString);
            return builder;
        }
    }

    //public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    //{
    //    public ApplicationDbContext CreateDbContext(string[] args)
    //    {
    //        IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile(@Directory.GetCurrentDirectory() + "../../noHRforIT/appsettings.json").Build();
    //        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
    //        var connectionString = configuration.GetConnectionString("noHRConnectionString");
    //        builder.UseSqlServer(connectionString);
    //        return new ApplicationDbContext(builder.Options);
    //    }
    //}
}
