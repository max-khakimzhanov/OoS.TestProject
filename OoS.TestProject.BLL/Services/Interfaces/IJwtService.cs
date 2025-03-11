using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OoS.TestProject.BLL.Services.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(IEnumerable<Claim> claims);
    }
}
