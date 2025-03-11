using OoS.TestProject.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OoS.TestProject.BLL.Services.Interfaces
{
    public interface IClaimsService
    {
        Task<List<Claim>> CreateClaimsAsync(User user);
    }
}
