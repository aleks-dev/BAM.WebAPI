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
        Task ApplyLoanTransactionAsync(int accountId, Loan loan, decimal amount);
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

        public async Task ApplyLoanTransactionAsync(int accountId, Loan loan, decimal amount)
        {
            if (loan is null)
                throw new ArgumentNullException(nameof(loan));
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            // TransactionScope ensures both operations are committed or rolled back together.
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                await _loanRepo.IncreaseAccountBalanceByAmountAsync(accountId, amount).ConfigureAwait(false);
                _logger.LogDebug("Increased account balance by {Amount} for account {AccountId}", amount, accountId);

                await _loanRepo.AddAsync(loan).ConfigureAwait(false);
                _logger.LogDebug("Added loan entity for account {AccountId} with amount {Amount}", accountId, amount);

                scope.Complete();
                _logger.LogInformation("UnitOfWork transaction completed for account {AccountId}", accountId);
            }
            catch
            {
                _logger.LogWarning("UnitOfWork transaction failed for account {AccountId}; rolling back", accountId);
                throw;
            }
        }
    }


}