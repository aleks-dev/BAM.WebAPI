using AutoMapper;
using BAM.Domain.Models;
using BAM.Infra.Entities;

namespace BAM.DataAccessLayer.Mapping
{
    public class LoanMappingProfile : Profile
    {
        public LoanMappingProfile()
        {
            CreateMap<LoanEntity, Loan>()
                .ForMember(loan => loan.Account, opt => opt.Ignore())
                .ReverseMap();
        }
    }
}
