using BAM.Domain.Models;

namespace BAM.DataAccessLayer.Interfaces
{
    public interface ILoanRepo : IRepo<Loan>
    {
        Task IncreaseAccountBalanceByAmountAsync(int accountId, decimal amount);
    }
}
