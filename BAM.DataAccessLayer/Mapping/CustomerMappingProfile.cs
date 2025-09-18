using AutoMapper;
using BAM.Domain.Models;
using BAM.Infra.Entities;

namespace BAM.DataAccessLayer.Mapping
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<CustomerEntity, Customer>()
                .ReverseMap()
                ;
        }
    }
}
