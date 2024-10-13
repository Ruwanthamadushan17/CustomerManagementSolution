using AutoMapper;
using CustomerAPI.DTOs;
using CustomerAPI.Entities;

namespace CustomerAPI.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CustomerEnt, CustomerResponseDto>();
            CreateMap<CustomerRequestDto, CustomerEnt>();
        }
    }
}
