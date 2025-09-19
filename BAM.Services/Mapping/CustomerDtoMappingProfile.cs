using AutoMapper;
using BAM.Contracts.DTO;
using BAM.Domain.Models;

namespace BAM.WebAPI.Mapping
{
    public class CustomerDtoMappingProfile : Profile
    {
        public CustomerDtoMappingProfile()
        {
            CreateMap<Customer, CustomerDto>()
                .ReverseMap();
        }
    }
}

