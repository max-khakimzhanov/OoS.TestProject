using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OoS.TestProject.DAL.Persistence;
using OoS.TestProject.DAL.Repositories.Interfaces;
using OoS.TestProject.DAL.Repositories.Realizations;
using FluentValidation;
using FluentValidation.AspNetCore;
using OoS.TestProject.BLL.Validation.Validators.DtoValidators.Student;

namespace OoS.TestProject.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
        }

        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddRepositoryServices();
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            services.AddAutoMapper(currentAssemblies);

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<CreateStudentDtoValidator>();
        }
         
        public static void AddApplicationServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            var mariaConnectionString = configuration.GetConnectionString("MariaDbConnection");

            services.AddDbContext<OoSTestProjectDbContext>(options =>
                options.UseMySql(mariaConnectionString, ServerVersion.AutoDetect(mariaConnectionString)));

            services.AddCustomServices();
            services.AddControllers();
        }

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
