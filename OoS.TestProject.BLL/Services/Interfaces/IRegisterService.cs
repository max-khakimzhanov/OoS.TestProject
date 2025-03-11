using FluentResults;
using OoS.TestProject.BLL.Dto.User;

namespace OoS.TestProject.BLL.Services.Interfaces
{
    public interface IRegisterService
    {
        Task<Result<string>> RegisterAsync(RegisterDto registerDto);
    }
}
