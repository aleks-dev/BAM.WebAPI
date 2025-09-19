using AutoMapper;
using BAM.DataAccessLayer.Interfaces;
using BAM.Domain.Models;
using BAM.Infra.Database;
using BAM.Infra.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BAM.DataAccessLayer.Repos
{
    public class LoanRepo : ILoanRepo
    {
        private readonly BAMDbContext _ctx;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanRepo> _logger;

        public LoanRepo(BAMDbContext ctx, IMapper mapper, ILogger<LoanRepo> logger)
        {
            _ctx = ctx; _mapper = mapper; _logger = logger;
        }

        public async Task AddAsync(Loan loan)
        {
            throw new NotImplementedException();
        }

        public async Task<Loan?> GetByIdAsync(int id)
        {
            _logger.LogInformation("GetAsync called for id {Id}", id);
            var any = await _ctx.Loans.Where(u => u.Id == id).AnyAsync();
            if (!any)
            {
                _logger.LogInformation("GetAsync: no loan found for Id {Id}", id);
                return null;
            }

            var entity = await _ctx.Loans.Where(u => u.Id == id).FirstAsync();
            _logger.LogInformation("GetAsync: found loan for id {Id}", id);
            return _mapper.Map<Loan>(entity);
        }

        public async Task<IList<Loan>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Loan loan)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task IncreaseAccountBalanceByAmountAsync(int accountId, decimal amount)
        {
            _logger.LogInformation("IncreaseAccountBalanceByAmountAsync called for loan on account {Id}", accountId);
            try
            {
                if (amount <= 0)
                {
                    _logger.LogError("Loan amount must be positive");
                    throw new ArgumentOutOfRangeException(nameof(amount), "Loan  amount must be positive.");
                }

                var account = await _ctx.Accounts.FirstOrDefaultAsync(u => u.Id == accountId);
                if (account != null)
                {
                    account.Balance += amount;
                }
                else
                {
                    _logger.LogError("Account id is not valid");
                    throw new ArgumentException("Account id is not valid");
                }

                await _ctx.SaveChangesAsync();
                _logger.LogInformation("IncreaseAccountBalanceByAmountAsync: saved changes for account {accountId}", accountId);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
