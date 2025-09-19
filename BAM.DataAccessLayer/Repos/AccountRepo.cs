using AutoMapper;
using BAM.DataAccessLayer.Interfaces;
using BAM.Domain.Models;
using BAM.Infra.Database;
using BAM.Infra.Entities;
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

        public async Task<Account?> GetByIdAsync(int id)
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

        public async Task UpdateAsync(Account account)
        {
            _logger.LogInformation("UpdateAsync called for id {Id}", account?.Id);
            if (account != null)
            {
                try
                {
                    var existing = await _ctx.Accounts.FirstOrDefaultAsync(u => u.Id == account.Id);
                    if (existing != null)
                    {
                        _logger.LogDebug("UpdateAsync: mapping incoming account onto tracked entity for id {Id}", account.Id);
                        _mapper.Map(account, existing);
                    }
                    else
                    {
                        _logger.LogDebug("UpdateAsync: attaching new entity for id {Id}", account.Id);
                        var newEntity = _mapper.Map<AccountEntity>(account);
                        _ctx.Accounts.Attach(newEntity);
                        _ctx.Entry(newEntity).State = EntityState.Modified;
                    }

                    await _ctx.SaveChangesAsync();
                    _logger.LogInformation("UpdateAsync: saved changes for id {Id}", account.Id);
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "UpdateAsync: error updating account id {Id}", account.Id);
                    throw;
                }
            }
            else
            {
                _logger.LogWarning("UpdateAsync called with null account");
            }
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
            var list = await _ctx.Accounts.Where(a => a.CustomerId == customerId).AsNoTracking().ToListAsync();
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

        public async Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
