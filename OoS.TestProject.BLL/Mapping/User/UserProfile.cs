using AutoMapper;
using OoS.TestProject.BLL.Dto.User;
using UserEntity = OoS.TestProject.DAL.Entities.User;

namespace OoS.TestProject.BLL.Mapping.User
{
    public class UserProfile : Profile
    {
        public UserProfile() 
        {
            CreateMap<RegisterDto, UserEntity>();
        }
    }
}
