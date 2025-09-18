using BAM.Domain.Models;

namespace BAM.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IList<Account>> GetAccountsByCustomerIdAsync(int customerId);
        Task TransferMoneyBetweenAccountsAsync(int fromAccountId, int toAccountId, decimal amount);
    }
}
