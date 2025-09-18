using AutoMapper;
using BAM.Contracts.DTO;
using BAM.Domain.Models;

namespace BAM.WebAPI.Mapping
{
    public class AccountDtoMappingProfile : Profile
    {
        public AccountDtoMappingProfile()
        {
            CreateMap<Account, AccountDto>()
                .ReverseMap();
        }
    }
}
