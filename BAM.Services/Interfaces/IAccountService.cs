using BAM.Contracts.DTO;

namespace BAM.Services.Interfaces
{
    public interface IAccountService
    {
        Task<IList<AccountDto>> GetAccountsByCustomerIdAsync(int customerId);
        Task TransferMoneyBetweenAccountsAsync(TransferDto transfer);
    }
}
