using Microsoft.OpenApi.Models;

namespace OoS.TestProject.WebApi.Extensions
{
    public static class SwaggerConfiguration
    {
        public static void AddSwaggerServices(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "OoS.TestProject.WebApi", Version = "v1" });
            });
        }
    }
}
