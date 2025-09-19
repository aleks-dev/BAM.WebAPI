using BAM.DataAccessLayer.Interfaces;
using BAM.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace BAM.DataAccessLayer.UnitOfWork
{
    public interface IUnitOfWork
    {
        /// <summary>
        /// Performs IncreaseAccountBalanceByAmountAsync and AddAsync(loan) as a single transactional operation.
        /// Throws exceptions on failure so callers can decide how to handle them.
        /// </summary>
        Task ApplyForLoanAsync(Loan loan);
    }

    public class ApplyForLoanUnitOfWork : IUnitOfWork
    {
        private readonly ILoanRepo _loanRepo;
        private readonly ILogger<ApplyForLoanUnitOfWork> _logger;

        public ApplyForLoanUnitOfWork(ILoanRepo loanRepo, ILogger<ApplyForLoanUnitOfWork> logger)
        {
            _loanRepo = loanRepo ?? throw new ArgumentNullException(nameof(loanRepo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ApplyForLoanAsync(Loan loan)
        {
            if (loan is null)
                throw new ArgumentNullException(nameof(loan));
            if (loan.Amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(loan.Amount));

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                await _loanRepo.IncreaseAccountBalanceByAmountAsync(loan.Account.Id, loan.Amount);
                _logger.LogDebug("Increased account balance by {Amount} for account {AccountId}", loan.Amount, loan.Account.Id);

                await _loanRepo.AddAsync(loan);
                _logger.LogDebug("Added loan entity for account {AccountId} with amount {Amount}", loan.Account.Id, loan.Amount);

                scope.Complete();
                _logger.LogInformation("UnitOfWork transaction completed for account {AccountId}", loan.Account.Id);
            }
            catch
            {
                _logger.LogWarning("UnitOfWork transaction failed for account {AccountId}; rolling back", loan.Account.Id);
                throw;
            }
        }
    }


}