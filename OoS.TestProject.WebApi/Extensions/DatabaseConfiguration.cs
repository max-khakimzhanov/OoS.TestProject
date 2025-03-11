using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OoS.TestProject.DAL.Entities;
using OoS.TestProject.DAL.Persistence;

namespace OoS.TestProject.WebApi.Extensions
{
    public static class DatabaseConfiguration
    {
        public static void AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MariaDbConnection");

            services.AddDbContext<OoSTestProjectDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = true;
            })
            .AddEntityFrameworkStores<OoSTestProjectDbContext>()
            .AddDefaultTokenProviders();
        }
    }
}
