using OoS.TestProject.DAL.Repositories.Interfaces;
using OoS.TestProject.DAL.Repositories.Realizations;
using FluentValidation;
using FluentValidation.AspNetCore;
using OoS.TestProject.BLL.Validation.Validators.DtoValidators.Student;
using OoS.TestProject.BLL.Services.Interfaces;
using OoS.TestProject.BLL.Services.Realizations;

namespace OoS.TestProject.WebApi.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
            services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ITeacherRepository, TeacherRepository>();
        }

        public static void AddCustomServices(this IServiceCollection services)
        {
            services.AddRepositoryServices();
            var currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            services.AddAutoMapper(currentAssemblies);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(currentAssemblies));
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<CreateStudentDtoValidator>();

            services.AddScoped<IClaimsService, ClaimsService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IRegisterService, RegisterService>();
            services.AddScoped<ILoginService, LoginService>();
        }
    }
}
