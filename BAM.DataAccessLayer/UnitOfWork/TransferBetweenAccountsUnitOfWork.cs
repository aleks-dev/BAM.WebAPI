using BAM.DataAccessLayer.Interfaces;
using BAM.Domain.Models;
using Microsoft.Extensions.Logging;
using System.Transactions;

namespace BAM.DataAccessLayer.UnitOfWork
{
    public interface ITransferBetweenAccountsUnitOfWork
    {
        Task TransferAsync(Account fromAccount, Account toAccount, decimal amount);
    }

    public class TransferBetweenAccountsUnitOfWork : ITransferBetweenAccountsUnitOfWork
    {
        private readonly IAccountRepo _repo;
        private readonly ILogger<TransferBetweenAccountsUnitOfWork> _logger;

        public TransferBetweenAccountsUnitOfWork(IAccountRepo repo, ILogger<TransferBetweenAccountsUnitOfWork> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task TransferAsync(Account fromAccount, Account toAccount, decimal amount)
        {
            _logger.LogInformation("AccountUnitOfWork: TransferAsync called: from {fromAccountId} to {toAccountId} amount {amount}",
                fromAccount.Id, toAccount.Id, amount);

            fromAccount.Withdraw(amount);
            toAccount.Deposit(amount);

            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            try
            {
                await _repo.UpdateAsync(fromAccount);
                await _repo.UpdateAsync(toAccount);
            }
            catch
            {
                _logger.LogWarning("UnitOfWork transaction failed from account {fromAccountId} to {toAccountId}; rolling back", fromAccount.Id, toAccount.Id);
                throw;
            }
        }
    }
}