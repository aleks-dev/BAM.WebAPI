using BAM.DataAccessLayer.Interfaces;
using BAM.Domain.Models;
using BAM.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace BAM.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepo _repo;
        private readonly ILogger<AccountService> _logger;

        public AccountService(IAccountRepo repo, ILogger<AccountService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<IList<Account>> GetAccountsByCustomerIdAsync(int customerId)
        {
            _logger.LogInformation("AccountService: GetAccountsByCustomerIdAsync called for customerId {customerId}", customerId);
            return await _repo.GetAllByCustomerId(customerId);
        }

        public async Task TransferMoneyBetweenAccountsAsync(int fromAccountId, int toAccountId, decimal amount)
        {
            _logger.LogInformation("AccountService: TransferMoneyBetweenAccountsAsync called for accounts: from {fromAccountId} to {toAccountId}", fromAccountId, toAccountId);
            await _repo.TransferMoneyBetweenAccountsAsync(fromAccountId, toAccountId, amount);
        }
    }
}
