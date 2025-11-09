using AutoMapper;
using Buildify.Core.DTOs;
using Buildify.Core.Entities.Identity;

namespace Buildify.APIs.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Address, AddressDto>().ReverseMap();
        CreateMap<CreateAddressDto, Address>();
        CreateMap<UpdateAddressDto, Address>();
        CreateMap<AppUser, AccountProfileDto>();
    }
}
