using BAM.Contracts.DTO;

namespace BAM.Services.Interfaces
{
    public interface ILoanService
    {
        Task<bool> ApplyForLoanAsync(LoanApplicationDto loanApplication);
    }
}
