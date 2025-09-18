using AutoMapper;
using BAM.DataAccessLayer.Interfaces;
using BAM.Domain.Models;
using BAM.Infra.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BAM.DataAccessLayer.Repos
{
    public class AccountRepo : IAccountRepo
    {
        private readonly BAMDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountRepo> _logger;

        public AccountRepo(BAMDbContext ctx, IMapper mapper, ILogger<AccountRepo> logger)
        {
            _ctx = ctx; 
            _mapper = mapper; 
            _logger = logger;
        }

        public async Task<Account?> GetAsync(int id)
        {
            _logger.LogInformation("GetAsync called for id {Guid}", id);
            var any = await _ctx.Accounts.Where(u => u.Id == id).AnyAsync();
            if (!any)
            {
                _logger.LogInformation("GetAsync: no account found for Guid {Guid}", id);
                return null;
            }

            var entity = await _ctx.Accounts.Where(u => u.Id == id).FirstAsync();
            _logger.LogInformation("GetAsync: found account for id {Guid}", id);
            return _mapper.Map<Account>(entity);
        }

        public async Task TransferMoneyBetweenAccountsAsync(int fromAccountId, int toAccountId, decimal amount)
        {
            _logger.LogInformation("TransferMoneyBetweenAccounts called for accounts: from {fromAccountId} to {toAccountId}", fromAccountId, toAccountId);
            try
            {
                if (amount <= 0)
                {
                    _logger.LogError("Transfer amount must be positive");
                    throw new ArgumentOutOfRangeException(nameof(amount), "Transfer amount must be positive.");
                }

                var from = await _ctx.Accounts.FirstOrDefaultAsync(u => u.Id == fromAccountId);
                var to = await _ctx.Accounts.FirstOrDefaultAsync(u => u.Id == toAccountId);

                if (from != null && to != null)
                {
                    if (from.Balance < amount)
                    {
                        _logger.LogError("Insufficient funds in account id {fromAccountId}", fromAccountId);
                        throw new InvalidOperationException("Insufficient funds in the source account.");
                    }

                    from.Balance -= amount;
                    to.Balance += amount;
                }
                else
                {
                    _logger.LogError("At least 1 account id is not valid");
                    throw new ArgumentException("At least 1 account id is not valid");
                }

                await _ctx.SaveChangesAsync();
                _logger.LogInformation("TransferMoneyBetweenAccounts: saved changes for for accounts: from {fromAccountId} to {toAccountId}", fromAccountId, toAccountId);
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<IList<Account>> GetAllByCustomerId(int customerId)
        {
            _logger.LogInformation("GetAllByCustomerId called");
            var list = (await _ctx.Customers.Where(a => a.Id == customerId).AsNoTracking().FirstAsync())?.Accounts.ToList();
            _logger.LogInformation("GetAllByCustomerId: returning {Count} accounts", list?.Count);
            return _mapper.Map<IList<Account>>(list);
        }

        #region Not Implemented
        // The following methods are not implemented as they are not needed for the current requirements.
        public async Task AddAsync(Account account)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<Account>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Account account)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
