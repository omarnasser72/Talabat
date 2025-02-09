using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;

namespace Talabat.APIs.Profiles
{
    public class UserProfile : BaseProfile
    {
        public UserProfile()
        {
            CreateMap<AppUser, UserDTO>();

        }
    }
}
