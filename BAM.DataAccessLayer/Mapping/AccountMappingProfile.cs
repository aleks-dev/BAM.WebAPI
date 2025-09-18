using AutoMapper;
using BAM.Domain.Models;
using BAM.Infra.Entities;

namespace BAM.DataAccessLayer.Mapping
{
    public class AccountMappingProfile : Profile
    {
        public AccountMappingProfile()
        {
            CreateMap<AccountEntity, Account>()
                .ReverseMap();
        }
    }
}
