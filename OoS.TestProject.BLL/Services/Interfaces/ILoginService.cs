using FluentResults;
using OoS.TestProject.BLL.Dto.User;

namespace OoS.TestProject.BLL.Services.Interfaces
{
    public interface ILoginService
    {
        Task<Result<string>> LoginAsync(LoginDto loginDto);
    }
}
