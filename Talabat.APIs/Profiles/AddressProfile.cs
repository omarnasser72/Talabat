using Talabat.APIs.DTOs;
using Talabat.APIs.Models;
using Talabat.Core.Entities;

namespace Talabat.APIs.Profiles
{
    public class AddressProfile : BaseProfile
    {
        public AddressProfile()
        {
            CreateMap<Address, AddressDTO>();
            CreateMap<AddressModel, Address>();
        }
    }
}
